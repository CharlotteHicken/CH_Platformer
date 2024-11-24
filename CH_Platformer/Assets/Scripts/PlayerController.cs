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

    public float moveSpeed;
    float gravity;
    float jumpVelocity;
    public float apexHeight;
    public float apexTime;
   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gravity = 2 * apexHeight / (Mathf.Pow(apexTime, 2));
        rb.gravityScale = gravity;
        jumpVelocity = 2 * apexHeight / apexTime;
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), rb.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded())
        {
            playerInput.y = jumpVelocity;
        }
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.velocity = new Vector2(playerInput.x * moveSpeed, playerInput.y);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
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
