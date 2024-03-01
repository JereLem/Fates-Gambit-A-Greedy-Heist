using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Variables for movement
    private float horizontal;
    private float vertical;

    [Header("Player speeds")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float climbingSpeed = 3f;
    [SerializeField] private float jumpingPower = 7f;
    [SerializeField] public int jumpsLeft = 1;

    [Header("Gravity and Jumping")]
    [SerializeField] private const float noGravity = 0f;
    [SerializeField] private const float gravity = 1f;
    private bool isClimbing = false;
    private const float maxJumpTime = 0.2f;

    [Header("Ground and Wall Checks")]
    private const float groundCheckDistance = 2f;
    private const float climbCheckDistance = 0.2f;

    [Header("Gameobjects")]
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask climbableLayer;
    
    [Header("Hookshot")]
    private Camera cam;
    private RaycastHit2D hit;
    private LineRenderer lr;
    private bool OnGrappling = false;
    private Vector3 spot;
    [SerializeField] private LayerMask GrapplingObj;
    [SerializeField] public float hookMoveSpeed = 10f;

    // Variables to track whether the player is using the hookshot
    private bool isUsingHookshot = false;
    public bool enableHookshot = false;   

    // Animations
    public Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    [Header("UI & Icons")]
    private Transform panel;
    private Color originalColor;
    private Color grayedOutColor;
    public Image highlightHook;


    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        panel = GameObject.Find("LowerPanel").transform;
        highlightHook = panel.Find("HighlightHook").GetComponent<Image>();
        originalColor = highlightHook.color;
        grayedOutColor = Color.gray;

        highlightHook.color = grayedOutColor;
    }

    void Update()
    {
        // PlayerMovement Update
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        //animator.SetFloat("Velocity", rb.velocity.y);

        FlipSprite(horizontal);

        if (Input.GetButtonDown("Jump"))
        {
            // Start a coroutine to handle short press for jumping
            StartCoroutine(JumpCoroutine());
        }

        if (Input.GetButton("Jump") && CanClimb())
        {
            // Handle climbing while the button is held
            isClimbing = true;
            player.gravityScale = noGravity;
            jumpsLeft = 1;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
        // Toggle the enableHookshot variable when the 'F' key is pressed
        enableHookshot = !enableHookshot;

        // Reset the isUsingHookshot variable when disabling the hookshot
        if (!enableHookshot)
        {
            isUsingHookshot = false;
        }
        }

        if (enableHookshot){
            // RopeAction Update
            if (Input.GetMouseButton(0))
            {
                RopeShoot();
                isUsingHookshot = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndShoot();
                isUsingHookshot = false;
            }
        }

        if (OnGrappling && enableHookshot)
        {
            QuickMove();
        }
        highlightHook.color = enableHookshot ? originalColor : grayedOutColor;
        DrawRope();
    }
    void FlipSprite(float horizontalInput)
    {
        // Check if the player is moving to the right
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false; // Don't flip the sprite
        }
        // Check if the player is moving to the left
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true; // Flip the sprite horizontally
        }
    }

    void FixedUpdate()
    {
        // Disable base movement if the hookshot is being used
        if (!isUsingHookshot)
            {
            if (isClimbing)
            {
                // Allow vertical movement while climbing
                player.velocity = new Vector2(horizontal * speed, vertical * climbingSpeed);
            }
            else
            {
                // Regular horizontal movement when not climbing
                player.velocity = new Vector2(horizontal * speed, player.velocity.y);
            }
        }
    }

    private IEnumerator JumpCoroutine()
    {
        // Differentiate between pressing and holding spacebar
        float timePressed = 0f;

        while (timePressed < maxJumpTime && Input.GetButton("Jump"))
        {
            timePressed += Time.deltaTime;
            yield return null;
        }

        if (timePressed < maxJumpTime && CanJump())
        {
            Jump();
            animator.SetTrigger("jumped");
        }
    }

    public bool IsGrounded()
    {
        Collider2D collision = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        animator.SetTrigger("landed");
        return !isClimbing && collision != null;
    }

    private bool CanJump()
    {
        // Check if the player can jump (only if on ground or wall)
        return (jumpsLeft > 0 && IsGrounded()) || (isClimbing && jumpsLeft > 0);
    }

    private void Jump()
    {
        if (isClimbing)
        {
            player.velocity = new Vector2(horizontal * speed, jumpingPower);
            isClimbing = false;
            player.gravityScale = gravity;
        }
        else
        {
            player.velocity = new Vector2(player.velocity.x, jumpingPower);
        }

        jumpsLeft--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            jumpsLeft = 1;
        }

        // Ensure player layer changes to be inside the balcony/house
        if (collision.gameObject.CompareTag("Balcony"))
        {
            {
                spriteRenderer.sortingOrder =  2;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ensure player falls if they go out of climbable area
        if (collision.gameObject.CompareTag("Climbable"))
        {
            {
                isClimbing = false;
                player.gravityScale = gravity;
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            {
                jumpsLeft = 0;
            }
        }
        
        // Ensure player layer changes to be outside the balcony/house
        if (collision.gameObject.CompareTag("Balcony"))
        {
            {
                spriteRenderer.sortingOrder = 3;

            }
        }
    }

    private bool CanClimb()
    {
        // Check if player can climb
        return !isClimbing && Physics2D.OverlapCircle(groundCheck.position, climbCheckDistance, climbableLayer);
    }

    // Rope Action methods
    void RopeShoot()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Control the distance between camera
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);

        // Detect the collision between player's position and mouse's using Raycast
        hit = Physics2D.Raycast(player.position, mouseWorldPosition - (Vector2)player.position, 100f, GrapplingObj);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Balcony"))
            {
                Debug.Log("Balcony!");
                OnGrappling = true;
                lr.positionCount = 2;
                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, hit.point);
            }
        }
    }

    void EndShoot()
    {
        OnGrappling = false;
        lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (OnGrappling)
        {
            lr.SetPosition(0, this.transform.position);
        }
    }

    void QuickMove()
    {
        if (hit.collider != null)
        {
            StartCoroutine(MovePlayerSmoothly(hit.point));
        }
    }

    IEnumerator MovePlayerSmoothly(Vector2 targetPosition)
    {
        // Introduced threshold to stop player adjusting to the targetPosition for too long
        float distanceThreshold = 0.1f;
        float distance = Vector2.Distance(player.position, targetPosition);
        float duration = distance / hookMoveSpeed;

        float startTime = Time.time;

        while (Time.time - startTime < duration && distance > distanceThreshold)
        {
            float step = hookMoveSpeed * Time.deltaTime;
            player.position = Vector2.Lerp(player.position, targetPosition, step / distance);
            distance = Vector2.Distance(player.position, targetPosition); // Update the distance

            // Check if within threshold
            if (distance <= distanceThreshold)
            {
                player.position = targetPosition;
            }

            Debug.Log("Distance: " + distance + " duration: " + duration + " left time: " + (Time.time - startTime));
            yield return null;
        }

        Debug.Log("QuickMove completed");
    }

}
