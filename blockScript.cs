using UnityEngine;

public class blockScript : MonoBehaviour
{
    public Transform pointA;     // Start point
    public Transform pointB;     // End point
    public float speed = 2f;     // Movement speed

    private Vector3 target;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Assign both pointA and pointB in the Inspector.");
            enabled = false;
            return;
        }

        target = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Switch target when reaching one point
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
}
