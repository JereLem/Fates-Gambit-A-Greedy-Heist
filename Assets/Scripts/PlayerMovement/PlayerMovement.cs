using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal; // Horizontal input for movement control
    private float vertical; // Vertical input for climbing control
    private float speed = 5f; // Moving speed left/right 
    private float climbingSpeed = 3f; // Climbing speed
    private float jumpingPower = 7f; // Jumping speed
    private int jumpsLeft = 2; // Maximum number of jumps
    private bool isClimbing = false;

    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask climbableLayer;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && CanJump())
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
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool CanJump()
    {
        return (jumpsLeft > 0 && IsGrounded()) || (isClimbing && jumpsLeft > 0);
    }

    private void Jump()
    {
        if (isClimbing)
        {
            // If climbing, jump away from the wall
            player.velocity = new Vector2(horizontal * speed, jumpingPower);
            isClimbing = false;
            player.gravityScale = 1f;
        }
        else
        {
            // If not climbing, regular jump
            player.velocity = new Vector2(player.velocity.x, jumpingPower);
        }

        jumpsLeft--;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            jumpsLeft = 2; // Reset jumps when landing on the ground
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player is in contact with a climbable surface
        if (collision.gameObject.CompareTag("Climbable"))
        {
            isClimbing = true;
            player.gravityScale = 0f; // Disable gravity while climbing
            // Reset jumps
            jumpsLeft = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {
            isClimbing = false;
            player.gravityScale = 1f; // Restore gravity when not climbing
        }
    }
}
