using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Speed and forces")]
    public float moveSpeed = 5.0f;
    public float acceleration = 10f;
    public float movingDrag = 1f;
    public float stoppedDrag = 10f;
    public float jumpForce = 2.0f;
    public float floatSpeed = 7.0f;
    public float smoothness = 0.2f;

    private bool jump = false;
    private bool canFloat = true;
    [SerializeField] private bool isGrounded = false;

    // [Header("Animator")]
    // public Animator playerAnim;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Capture jump input in Update for responsiveness
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        //movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool floatUp = Input.GetKey(KeyCode.LeftShift);
        Debug.Log("Velocity: " + rb.linearVelocity); // Changed here

        //check if grounded
        if (Physics.Raycast(transform.position, Vector3.down, 0.7f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Jump
        if (jump && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            jump = false;
        }

        // Float
        if (floatUp && !isGrounded && canFloat)
        {
            Debug.Log("Floating");
            canFloat = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, floatSpeed, rb.linearVelocity.z); // Changed here
            StartCoroutine(Float());
            // playerAnim.SetBool("canFloat", false);
        }

        Vector3 input = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 targetVelocity = input * moveSpeed;
        if (input.magnitude > 0.1f)
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z); // Changed here
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Changed here
        }
    }

    IEnumerator Float()
    {
        // playerAnim.SetBool("canFloat", true);
        yield return new WaitForSeconds(4f);
        // playerAnim.SetBool("canFloat", false);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Changed here
        canFloat = true;
    }
}


