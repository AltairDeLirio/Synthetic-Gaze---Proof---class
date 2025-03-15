using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    [Header("Movement")]
    public float moveSpeed = 5f;

    float horizontalMovement;
    [Header("Jumping")]
    public float jumpPower = 7f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMult = 2f;

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        GroundCheck();
        Gravity();

        if (horizontalMovement < 0)
        {
            animator.SetBool("WalkLeft", true); 
            animator.SetBool("WalkRight", false); 
        }
        else if (horizontalMovement > 0)
        {
            animator.SetBool("WalkLeft", false); 
            animator.SetBool("WalkRight", true); 
        }
        else
        {
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", false);
        }
        if (rb.linearVelocity.y > 0) 
        {
            if (horizontalMovement < 0)
            {
                animator.SetBool("JumpLeft", true);  
                animator.SetBool("JumpRight", false); 
            }
            else if (horizontalMovement > 0)
            {
                animator.SetBool("JumpLeft", false); 
                animator.SetBool("JumpRight", true); 
            }
            else
            {
                
                if (transform.localScale.x < 0) 
                {
                    animator.SetBool("JumpLeft", true);
                    animator.SetBool("JumpRight", false);
                }
                else
                {
                    animator.SetBool("JumpLeft", false);
                    animator.SetBool("JumpRight", true);
                }
            }
        }
        else if (rb.linearVelocity.y < 0)
        {
            if (horizontalMovement < 0)
            {
                animator.SetBool("JumpLeft", true);
                animator.SetBool("JumpRight", false);
            }
            else if (horizontalMovement > 0)
            {
                animator.SetBool("JumpLeft", false);
                animator.SetBool("JumpRight", true);
            }
        }
        else
        {
            animator.SetBool("JumpLeft", false);
            animator.SetBool("JumpRight", false);
        }

    }

    private void Gravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if(jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
            if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
            
        }
    }

    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            animator.SetBool("JumpLeft", false);
            animator.SetBool("JumpRight", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
