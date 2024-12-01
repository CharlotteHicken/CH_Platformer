using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }
    Rigidbody2D rb;
    PlayerController.FacingDirection lastDirection = FacingDirection.right;


    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;
    float gravity;
    float initialJumpSpeed;
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    public float maxVelocity = 15f;
    float terminalVelocity;
    public float slowFallVelocity = 3f;
    public float coyoteTime;
    float currentGroundTime;
    float accelerationRate;
    float decelerationRate;

    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    bool isGrounded = false;

    Vector2 velocity;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));

        initialJumpSpeed = 2 * apexHeight / apexTime;
        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        terminalVelocity = maxVelocity;

    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        rb.velocity = velocity;
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if(velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    public bool IsWalking()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            return true;
        }
        return false;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }
     private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);
    }
    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
            terminalVelocity = maxVelocity;

        }
        else if (!isGrounded && Input.GetButton("Jump"))
        {
            terminalVelocity = slowFallVelocity;
        }
        else
        {
            terminalVelocity = maxVelocity;
        }
        
        if (velocity.y < -terminalVelocity)
        {
            velocity.y = -terminalVelocity;
        }
    }
    public FacingDirection GetFacingDirection()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            lastDirection = FacingDirection.right;
            return FacingDirection.right;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            lastDirection = FacingDirection.left;
            return FacingDirection.left;
        }
        return lastDirection;
    }
}
