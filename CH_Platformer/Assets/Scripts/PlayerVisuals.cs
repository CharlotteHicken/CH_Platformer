using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private int isWalkingHash, isGroundedHash;

    // Start is called before the first frame update
    void Start()
    {
        isWalkingHash = Animator.StringToHash("IsWalking");
        isGroundedHash = Animator.StringToHash("IsGrounded");
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(isWalkingHash, playerController.IsWalking());
        animator.SetBool(isGroundedHash, playerController.IsGrounded());
        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
            default:
                bodyRenderer.flipX = false;
                break;
        }
    }
}
