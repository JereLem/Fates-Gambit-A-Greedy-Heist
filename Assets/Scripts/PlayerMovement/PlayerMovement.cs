using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

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
        }
    }

    private void FixedUpdate()
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

    private bool IsGrounded()
    {
        Collider2D collision = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
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
    }

    private bool CanClimb()
    {   
        // Check if player can climb
        return !isClimbing && Physics2D.OverlapCircle(groundCheck.position, climbCheckDistance, climbableLayer);
    }

}
