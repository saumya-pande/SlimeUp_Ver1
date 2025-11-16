using UnityEngine;

public class keyAnim : MonoBehaviour
{
    private Rigidbody rb;
    public float rotateSpeed = 90f;
    public float floatAmplitude = 0.2f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = rb.transform.position;
    }

    void Update()
    {
        // Rotate continuously
        rb.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        // Smooth up-and-down floating motion
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        rb.transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
