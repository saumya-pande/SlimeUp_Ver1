using UnityEngine;

public class StairInteraction : MonoBehaviour
{
    [Header("Movement Settings")]
    public float climbSpeed = 2f;
    public Transform upPosition;    // Assign position at top of stairs
    public Transform downPosition;  // Assign position at bottom of stairs
    
    [Header("Detection Settings")]
    public float lookAngle = 45f;   // How directly you need to look at stairs
    public string stairsTag = "Stairs";
    
    private bool isLookingAtStairs = false;
    private bool isCollidingWithStairs = false;
    private bool isMoving = false;
    private CharacterController controller;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // Check if looking at stairs with raycast
        CheckLookingAtStairs();
        
        // If looking at stairs AND colliding with it
        if (isLookingAtStairs && isCollidingWithStairs && !isMoving)
        {
            // Press W to go up
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(MoveToPosition(upPosition.position));
            }
            
            // Press S to go down
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(MoveToPosition(downPosition.position));
            }
        }
    }
    
    void CheckLookingAtStairs()
    {
        RaycastHit hit;
        
        // Cast ray from camera forward
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (hit.collider.CompareTag(stairsTag))
            {
                // Calculate angle between look direction and stairs
                Vector3 directionToStairs = (hit.point - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToStairs);
                
                isLookingAtStairs = angle < lookAngle;
                return;
            }
        }
        
        isLookingAtStairs = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(stairsTag))
        {
            isCollidingWithStairs = true;
            Debug.Log("Near stairs - Look at them and press W (up) or S (down)");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(stairsTag))
        {
            isCollidingWithStairs = false;
        }
    }
    
    System.Collections.IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;
        
        // Disable character controller temporarily
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Move to target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, climbSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Snap to exact position
        transform.position = targetPosition;
        
        // Re-enable character controller
        if (controller != null)
        {
            controller.enabled = true;
        }
        
        isMoving = false;
    }
}