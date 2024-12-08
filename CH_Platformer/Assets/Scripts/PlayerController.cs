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


    public float maxWalkSpeed = 5f;
    float maxSpeed = 5f;
    public float maxSprintSpeed = 10f;
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
    public float accelerationRate;
    float decelerationRate;

    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    bool isGrounded = false;

    bool ladderTime = false;
    bool climbing;

    Vector2 velocity;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));

        initialJumpSpeed = 2 * apexHeight / apexTime;
        accelerationRate = maxWalkSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        terminalVelocity = maxVelocity;

    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();

        //modifies the maxspeed that will then determine if the player is sprinting or not
        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxSpeed = maxSprintSpeed;
        }
        else
        {
            maxSpeed = maxWalkSpeed;
        }

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        //if player is colliding with the ladder, allow vertical control and the ability to climb. Otherwise the player cannot climb
        if (ladderTime)
        {
            playerInput.y = Input.GetAxisRaw("Vertical");
            if (playerInput.y != 0) //only set climbing to true when player is pressing an input
            {
                climbing = true;
            }
        }
        else
        {
            playerInput.y = 0;
            climbing = false;
        }


        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded && !climbing) //if player is not on the ground, or not climbing, apply gravity. if the player is climbing, move them based on player input. Otherwise, do not move player vertically.
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (climbing)
        {
            velocity.y = playerInput.y * maxSpeed;
        }
        else
        {
            velocity.y = 0;
        }

        rb.velocity = velocity; //apply the set velocites to the rigidbody
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(playerInput.x != 0) // if the player is pressing a horizontal movement button, change their position based on acceleration rate but clamp it to not go above max speed
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else //if the player is not pressing a horizontal movement button
        {
            if(velocity.x > 0) //if player was moving right, decrease their movement until it is zero
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0) //if player was moving left, add to their movement until it is zero
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    public bool IsWalking()
    {
        if (Input.GetAxis("Horizontal") != 0) //if player is pressing a button, plays walking animation
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
        isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask); //if physics box collides with the ground, player is grounded
    }
    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump")) //if player is on the ground and jump button is pressed, apply jump physics and set to not grounded.
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
            terminalVelocity = maxVelocity;

        }
        else if (!isGrounded && Input.GetButton("Jump")) //if player is still holding jump button while in the air, set change terminal velocity so they fall slower
        {
            terminalVelocity = slowFallVelocity;
        }
        else //if player is not holding space, set terminal veloctiy to a normal falling velocity
        {
            terminalVelocity = maxVelocity;
        }
        
        if (velocity.y < -terminalVelocity) //if velocity is over the terminal velocity, set it to the terminal velocity
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladderTime = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladderTime = false;
        }
    }
}
