using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PLAYERANIM
{
    IDLE = 0,
    RUN = 0,
    JUMP = 0,
    FALL = 0,
    BOUNCE = 1,
    THROW = 1
}

public class AnimationListener : MonoBehaviour
{
    #region REFERENCES
    private Animator playerAnim;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    #endregion

    #region FIELDS
    [SerializeField] private string currentState;

    [SerializeField] private Vector2 movementVector;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool landed;
    [SerializeField] private bool isBouncing;
    [SerializeField] private bool isThrowing;
    [SerializeField] private bool runThrow_R;

    private const string PLAYER_IDLE = "Player_Idle";
    private const string PLAYER_RUN = "Player_Run";
    private const string PLAYER_JUMP = "Player_Jump";
    private const string PLAYER_FALL = "Player_Fall";
    private const string PLAYER_LAND = "Player_Land";
    private const string PLAYER_BOUNCE = "Player_Bounce";
    private const string PLAYER_THROW = "Player_Throw";
    private const string PLAYER_THROW_R = "Player_Run_Throw_R";
    private const string PLAYER_THROW_L = "Player_Run_Throw_L";
    #endregion

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    void Update()
    {
        movementVector = movementEvent.GetMovementVector();
        isJumping = movementEvent.GetIsJumping();
        isGrounded = movementEvent.GetIsGrounded();
        isFalling = movementEvent.GetIsFalling();
        landed = movementEvent.GetIsLanding();
        isBouncing = movementEvent.GetIsBounceAnimating();
        isThrowing = mushroomEvent.GetThrowing();
        
        // Check for running/idle animations
        if(isGrounded & !isThrowing && !isBouncing)
        {
            if (Mathf.Abs(movementVector.x) >= 0.1)
            {
                ChangeAnimationState(PLAYER_RUN);

                // Check movement data for which leg Anari is on for throwing animations
                // If running, then check which leg the player is running on and update accordingly
                AnimatorClipInfo[] animationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
                AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
                int currentFrame = (int)(animationClip[0].clip.length * (animationInfo.normalizedTime % 1) * animationClip[0].clip.frameRate);
                if (currentFrame == 50 || (currentFrame >= 0 && currentFrame < 25))
                {
                    // Update for left foot
                    runThrow_R = false;
                }
                else if (currentFrame >= 25 && currentFrame < 50)
                {
                    // Update for right foot
                    runThrow_R = true;
                }
            } else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }

        // Check for jumping animation
        if(isJumping && !isGrounded)
        {
            ChangeAnimationState(PLAYER_JUMP);
        }

        // Check for bouncing animation
        if (isBouncing && !isThrowing && !isJumping)
        {
            ChangeAnimationState(PLAYER_BOUNCE);
        }

        // Check for falling animation
        if (isFalling && !isBouncing && !isGrounded)
        {
            ChangeAnimationState(PLAYER_FALL);
        }

        // Check for throwing animation
        if (isThrowing)
        {
            // End throw animation if in the middle of bouncing, jumping, or falling
            if(isBouncing || isJumping || isFalling)
            {
                EndThrowAnimation();
            } else
            {
                // If moving, check which leg to throw from
                if (Mathf.Abs(movementVector.x) >= 0.1)
                {
                    // Set the appropriate animation
                    if (runThrow_R)
                    {
                        ChangeAnimationState(PLAYER_THROW_R);
                    }
                    else
                    {
                        ChangeAnimationState(PLAYER_THROW_L);
                    }
                }
                else
                {
                    // Otherwise, throw in place
                    ChangeAnimationState(PLAYER_THROW);
                }
            }
        }
    }

    /// <summary>
    /// Change animation states
    /// </summary>
    /// <param name="newState">The new state to change to</param>
    private void ChangeAnimationState(string newState)
    {
        // Check if the current state is the same as the new state
        if(currentState == newState)
        {
            // If so, return to prevent repetition of frames and animations
            return;
        }

        // Play the new state
        playerAnim.Play(newState);

        // Set the current state to the new state
        currentState = newState;
    }

    /// <summary>
    /// End the bounce animation
    /// </summary>
    private void EndBounceAnimation()
    {
        isBouncing = false;
        movementEvent.SetIsBounceAnimating(false);
    }

    /// <summary>
    /// End the throw animatino
    /// </summary>
    private void EndThrowAnimation()
    {
        isThrowing = false;
        mushroomEvent.SetThrowing(false);
    }
}
