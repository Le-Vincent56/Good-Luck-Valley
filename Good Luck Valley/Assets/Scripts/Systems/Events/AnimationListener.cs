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
    [SerializeField] private int currentPriority = 0;

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

    private void OnEnable()
    {
        // Start listening to events
        //movementEvent.moveEvent.AddListener(RunningAnim);
        //movementEvent.jumpEvent.AddListener(JumpingAnim);
        //movementEvent.landEvent.AddListener(LandingAnim);
        //movementEvent.fallEvent.AddListener(FallingAnim);
        //movementEvent.bounceAnimationEvent.AddListener(BouncingAnim);
        //mushroomEvent.checkThrowAnimationEvent.AddListener(CheckThrowingAnim);
        //mushroomEvent.setThrowAnimationEvent.AddListener(SetThrowingAnim);
    }

    private void OnDisable()
    {
        // Stop listening to events
        //movementEvent.moveEvent.RemoveListener(RunningAnim);
        //movementEvent.jumpEvent.RemoveListener(JumpingAnim);
        //movementEvent.landEvent.RemoveListener(LandingAnim);
        //movementEvent.fallEvent.RemoveListener(FallingAnim);
        //movementEvent.bounceAnimationEvent.RemoveListener(BouncingAnim);
        //mushroomEvent.checkThrowAnimationEvent.RemoveListener(CheckThrowingAnim);
        //mushroomEvent.setThrowAnimationEvent.RemoveListener(SetThrowingAnim);
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
        if(isJumping)
        {
            ChangeAnimationState(PLAYER_JUMP);
        }

        // Check for falling animation
        if(isFalling && !isBouncing)
        {
            ChangeAnimationState(PLAYER_FALL);
        }

        // Check for bouncing animation
        if (isBouncing && !isThrowing)
        {
            ChangeAnimationState(PLAYER_BOUNCE);
        }

        // Check for throwing animation
        if (isThrowing && !isBouncing)
        {
            if (Mathf.Abs(movementVector.x) >= 0.1)
            {
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
                ChangeAnimationState(PLAYER_THROW);
            }

            AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if (animationInfo.normalizedTime % 1 > 0.9)
            {
                mushroomEvent.SetThrowing(false);
                isThrowing = false;
            }
        }
    }

    private void ChangeAnimationState(string newState)
    {
        if(currentState == newState)
        {
            return;
        }

        playerAnim.Play(newState);

        currentState = newState;
    }

    /// <summary>
    /// Set running animations based on data
    /// </summary>
    /// <param name="data">Movement data</param>
    private void RunningAnim()
    {
        // Set speed
        float speed = movementVector.x;

        // Set float based on movement data
        if (speed > 0.1 && isGrounded && currentPriority <= (int)PLAYERANIM.RUN)
        {
            ChangeAnimationState(PLAYER_RUN);
            currentPriority = (int)PLAYERANIM.RUN;

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
        }
        else if (isGrounded && currentPriority <= (int)PLAYERANIM.IDLE)
        {
            ChangeAnimationState(PLAYER_IDLE);
            currentPriority = (int)PLAYERANIM.IDLE;
        }
    }

    /// <summary>
    /// Set landing animations based on data
    /// </summary>
    private void LandingAnim()
    {
        // Set the landing bool based on data
        // landed = landData;

        //if(landed)
        //{
        //    ChangeAnimationState(PLAYER_LAND);
        //}
    }

    /// <summary>
    /// Set jumping animatinos based on data
    /// </summary>
    private void JumpingAnim()
    {
        if(isJumping && currentPriority <= (int)PLAYERANIM.JUMP)
        {
            ChangeAnimationState(PLAYER_JUMP);
            currentPriority = (int)PLAYERANIM.JUMP;
        }
    }

    /// <summary>
    /// Set falling animations based on data
    /// </summary>
    private void FallingAnim()
    {
        if(isFalling)
        {
            ChangeAnimationState(PLAYER_FALL);
            currentPriority = (int)PLAYERANIM.FALL;
        }
    }

    /// <summary>
    /// Set bouncing animations based on data
    /// </summary>
    private void BouncingAnim()
    {
        currentPriority = (int)PLAYERANIM.BOUNCE;

        if (isBouncing && currentPriority <= (int)PLAYERANIM.BOUNCE)
        {
            // If bouncing, set the trigger
            ChangeAnimationState(PLAYER_BOUNCE);
        }
    }

    /// <summary>
    /// Check the throwing animation to see when to end
    /// </summary>
    private void CheckThrowingAnim()
    {
        if (isThrowing)
        {
            AnimatorClipInfo[] animationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
            AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if (animationInfo.normalizedTime % 1 > 0.9)
            {
                mushroomEvent.SetThrowing(false);
                mushroomEvent.SetThrowAnim();
            }
        }

        // FOR WHEN THROW ANIMATIONS ARE FULLY IMPLEMENTED
        //if (throwPrepared)
        //{
        //    AnimatorClipInfo[] throwAnimationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
        //    int currentFrame = (int)(throwAnimationClip[0].weight * (throwAnimationClip[0].clip.length * throwAnimationClip[0].clip.frameRate));
        //    if (currentFrame == 5)
        //    {
        //        throwing = true;

        //        // Throw the shroom
        //        //switch (throwState)
        //        //{
        //        //    case ThrowState.Throwing:
        //        //        CheckShroomCount();
        //        //        throwState = ThrowState.NotThrowing;
        //        //        break;
        //        //}

        //        if (throwState == ThrowState.Throwing)
        //        {
        //            CheckShroomCount();
        //            throwState = ThrowState.NotThrowing;
        //        }

        //        // Reset throw variables
        //        canThrow = false;
        //        throwCooldown = 0.2f;
        //        bounceCooldown = 0.2f;
        //        throwPrepared = false;
        //    }
        //}
    }

    /// <summary>
    /// Set the throwing animation based on data
    /// </summary>
    private void SetThrowingAnim()
    {
        //if(isThrowing && currentPriority <= (int)PLAYERANIM.THROW)
        //{
        //    if(speed != 0)
        //    {
        //        if (runThrow_R)
        //        {
        //            ChangeAnimationState(PLAYER_THROW_R);
        //        }
        //        else
        //        {
        //            ChangeAnimationState(PLAYER_THROW_L);
        //        }
        //    } else
        //    {
        //        ChangeAnimationState(PLAYER_THROW);
        //    }

        //    currentPriority = (int)PLAYERANIM.THROW;
        //}
    }

    private void ResetPriority()
    {
        currentPriority = 0;
    }

    private void EndBounceAnimation()
    {
        isBouncing = false;
        movementEvent.SetIsBounceAnimating(false);
    }

    private void EndThrowAnimation()
    {
        isThrowing = false;
        mushroomEvent.SetThrowing(false);
    }
}
