using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);
    
    [Header("Movement Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float rotationSmoothing = 2f;
    
    [Header("Mouse Control")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxHorizontalAngle = 90f; // Left/Right limits
    [SerializeField] private float maxVerticalAngle = 80f;   // Up/Down limits
    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 0f;
    
    private Vector3 currentVelocity;
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    
    private void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (target == null)
                Debug.LogError("No player found! Please assign a target or tag your player as 'Player'");
        }

        // Initialize position
        if (target != null)
        {
            transform.position = target.position + offset;
            desiredPosition = transform.position;
            smoothedPosition = transform.position;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Handle mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update rotation angles
        currentHorizontalAngle += mouseX * mouseSensitivity;
        currentVerticalAngle -= mouseY * mouseSensitivity; // Inverted for natural feel

        // Clamp angles
        currentHorizontalAngle = Mathf.Clamp(currentHorizontalAngle, -maxHorizontalAngle, maxHorizontalAngle);
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -maxVerticalAngle, maxVerticalAngle);

        // Calculate rotation
        Quaternion rotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
        Vector3 rotatedOffset = rotation * offset;

        // Calculate and apply position
        desiredPosition = target.position + rotatedOffset;
        smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        transform.position = smoothedPosition;

        // Look at target
        transform.LookAt(target);
    }

    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(ShakeCameraCo(intensity, duration));
    }

    private System.Collections.IEnumerator ShakeCameraCo(float intensity, float duration)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            transform.localPosition = new Vector3(
                originalPosition.x + x,
                originalPosition.y + y,
                originalPosition.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}