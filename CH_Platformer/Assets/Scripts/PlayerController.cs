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
    float jumpVelocity;
    public float apexHeight;
    public float apexTime;
    public float terminalVelocity;
    public float coyoteTime;
    float currentGroundTime;
    float accelerationRate;
    float decelerationRate;

    Vector2 velocity;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = 2 * apexHeight / (Mathf.Pow(apexTime, 2));
        rb.gravityScale = gravity;
        jumpVelocity = 2 * apexHeight / apexTime;
        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;


    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), rb.velocity.y);

        if (!IsGrounded())
        {
            currentGroundTime = 0f;
        }
        else
        {
            currentGroundTime += Time.deltaTime;
        } 
        if (Input.GetKeyDown(KeyCode.Space) && (currentGroundTime <= coyoteTime || !IsGrounded()))
        {
            playerInput.y = jumpVelocity;
        }
        MovementUpdate(playerInput);

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

        if (rb.velocity.y < -terminalVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity);
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
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.5f,0.01f), 0f, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        if (hit)
        {
            return false;
        }
        return true;
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
