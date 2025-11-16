using UnityEngine;
using System.Collections;

public class slimeController : MonoBehaviour
{
    int damageAmount = 1;
    
    [Header("References")]
    private Rigidbody rb;
    public Transform groundCheck;
    public Transform cameraTransform;
    private AudioSource audioSource;
    public Animator anim;

    [Header("Sound Effects")]
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip floatSound;
    
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float airControl = 0.5f;
    public float acceleration = 15f;
    public float drag = 6f;
    
    [Header("Jumping")]
    public float jumpForce = 10f;
    private int canDoubleJump = 2;
    
    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    
    [Header("Balloon Float")]
    public float liftForce = 8f;
    public float targetHeight = 5f;
    public float hoverDuration = 3f;
    public float floatDrag = 2f;
    public float hoverStabilization = 0.5f;
    public float descentGravityMultiplier = 1.5f;
    
    [Header("Ground Detection")]
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public string groundTag = "Ground"; // Tag-based detection option
    
    // State tracking
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing = false;
    private bool isAscending = false;
    private bool isHovering = false;
    private bool canFloat = true;
    
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private float lastJumpTime = 0f;
    private float jumpSoundProtection = 0.3f;
    private float groundY; // Track actual ground level
    private float hoverStartTime;
    private float originalDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (groundCheck == null)
        {
            Debug.LogError("Assign Ground Check transform in inspector!");
        }
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        groundY = transform.position.y;
        originalDrag = drag;
        
        // Safety reset
        ResetFloatPhysics();
    }

    void Update()
    {
        // Check if player is dead
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsDead) return;

        // Input gathering
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        // Calculate movement direction relative to camera
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        moveDirection = (cameraForward * v + cameraRight * h).normalized;
        
        if (moveDirection.magnitude > 0.1f)
        {
            lastMoveDirection = moveDirection;
        }
        
        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump -= 1;
            }
            else if (canDoubleJump > 0)
            {
                Jump();
                canDoubleJump -= 1;
            }
        }
        
        // Dash input (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing && !isAscending && !isHovering)
        {
            StartCoroutine(Dash());
        }
        
        // Float input (E)
        if (Input.GetKeyDown(KeyCode.E) && canFloat && isGrounded)
        {
            StartCoroutine(BalloonCycle());
        }
    }

    void FixedUpdate()
    {
        // Check if player is dead
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsDead) return;

        // Ground check - using both layer and tag
        CheckGroundStatus();
        
        if (isGrounded)
        {
            canDoubleJump = 2;
            groundY = transform.position.y; // Update ground reference when actually grounded
        }
        
        // Handle balloon physics
        if (isAscending)
        {
            HandleAscent();
            ApplyMovement();
        }
        else if (isHovering)
        {
            HandleHover();
            ApplyMovement();
        }
        else if (!isDashing)
        {
            ApplyMovement();
            ApplyDrag();
        }
    }

    void CheckGroundStatus()
    {
        // Layer-based check
        bool layerCheck = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Tag-based check as backup
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius);
        bool tagCheck = false;
        
        foreach (Collider col in colliders)
        {
            if (col.CompareTag(groundTag))
            {
                tagCheck = true;
                break;
            }
        }
        
        isGrounded = layerCheck || tagCheck;
    }

    void ApplyMovement()
    {
        Vector3 targetVelocity = moveDirection * moveSpeed;
        float controlFactor = isGrounded ? 1f : airControl;
        
        Vector3 velocityChange = (targetVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z)) * controlFactor;
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);
        
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
        
        // Walk sound logic
        if (isGrounded && moveDirection.magnitude > 0.1f && !audioSource.isPlaying && walkSound != null)
        {
            if (Time.time - lastJumpTime > jumpSoundProtection)
            {
                audioSource.clip = walkSound;
                audioSource.Play();
            }
        }
        else if (moveDirection.magnitude < 0.1f || !isGrounded)
        {
            if (Time.time - lastJumpTime > jumpSoundProtection)
            {
                audioSource.Stop();
            }
        }
    }

    void ApplyDrag()
    {
        if (isGrounded)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 dragForce = -horizontalVelocity * drag;
            rb.AddForce(dragForce, ForceMode.Force);
        }
    }

    void Jump()
    {
        lastJumpTime = Time.time;
        
        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound);
            
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        
        if (dashSound != null)
            audioSource.PlayOneShot(dashSound);
        
        Vector3 dashDirection = moveDirection.magnitude > 0.1f ? moveDirection : lastMoveDirection;
        
        float originalGravity = rb.linearDamping;
        rb.linearDamping = 0;
        
        rb.linearVelocity = new Vector3(dashDirection.x * dashSpeed, rb.linearVelocity.y * 0.5f, dashDirection.z * dashSpeed);
        
        yield return new WaitForSeconds(dashDuration);
        
        rb.linearDamping = originalGravity;
        isDashing = false;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator BalloonCycle()
    {
        canFloat = false;
        float floatStartY = transform.position.y; // Local variable for this float instance
        
        // Start inflation animation
        if (anim != null)
            anim.SetBool("ispoofing", true);
        
        // Start playing float sound
        if (floatSound != null && audioSource != null)
        {
            audioSource.clip = floatSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Wait for inflation animation
        yield return new WaitForSeconds(0.5f);
        if (anim != null)
            anim.SetBool("ispoofing", false);

        // Disable gravity and start ascending
        rb.useGravity = false;
        rb.linearDamping = floatDrag;
        isAscending = true;

        // Rise until target height
        while (transform.position.y < floatStartY + targetHeight)
        {
            yield return null;
        }

        // Switch to hovering
        isAscending = false;
        isHovering = true;
        hoverStartTime = Time.time;

        // Hover for duration
        yield return new WaitForSeconds(hoverDuration);

        // Begin descent
        isHovering = false;
        rb.useGravity = true;
        
        if (anim != null)
            anim.SetBool("isDeflating", true);
        
        // Apply extra downward force instead of modifying global gravity
        float descentTimer = 0f;
        while (!isGrounded && descentTimer < 10f) // Timeout safety
        {
            rb.AddForce(Vector3.down * 9.81f * (descentGravityMultiplier - 1f), ForceMode.Acceleration);
            descentTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Wait one more frame to ensure we're stable on ground
        yield return new WaitForFixedUpdate();

        // Complete reset
        ResetFloatPhysics();
        
        if (anim != null)
            anim.SetBool("isDeflating", false);
        
        canFloat = true;
        Debug.Log("Balloon cycle complete - Reset to ground level: " + groundY);
    }

    void ResetFloatPhysics()
    {
        rb.useGravity = true;
        rb.linearDamping = originalDrag;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Preserve horizontal momentum
        isAscending = false;
        isHovering = false;
        
        // Stop float sound
        if (audioSource != null && audioSource.clip == floatSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    void HandleAscent()
    {
        rb.AddForce(Vector3.up * liftForce, ForceMode.Force);

        float heightProgress = (transform.position.y - groundY) / targetHeight;
        if (heightProgress > 0.7f)
        {
            rb.linearVelocity *= (1f - hoverStabilization * Time.fixedDeltaTime);
        }
    }

    void HandleHover()
    {
        float floatStartY = groundY; // Use actual ground level
        float heightDiff = (floatStartY + targetHeight) - transform.position.y;
        float stabilizingForce = heightDiff * 3f;
        
        rb.AddForce(Vector3.up * stabilizingForce, ForceMode.Force);
        rb.linearVelocity *= 0.95f;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damageAmount);
        }
    }
}