using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationListener : MonoBehaviour
{
    private Animator playerAnim;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Start listening to events
        movementEvent.moveEvent.AddListener(RunningAnim);
        movementEvent.jumpEvent.AddListener(JumpingAnim);
        movementEvent.landEvent.AddListener(LandingAnim);
        movementEvent.fallEvent.AddListener(FallingAnim);
        mushroomEvent.bounceAnimationEvent.AddListener(BouncingAnim);
        mushroomEvent.checkThrowAnimationEvent.AddListener(CheckThrowingAnim);
        mushroomEvent.setThrowAnimationEvent.AddListener(SetThrowingAnim);
    }

    private void OnDisable()
    {
        // Stop listening to events
        movementEvent.moveEvent.RemoveListener(RunningAnim);
        movementEvent.jumpEvent.RemoveListener(JumpingAnim);
        movementEvent.landEvent.RemoveListener(LandingAnim);
        movementEvent.fallEvent.RemoveListener(FallingAnim);
        mushroomEvent.bounceAnimationEvent.RemoveListener(BouncingAnim);
        mushroomEvent.checkThrowAnimationEvent.RemoveListener(CheckThrowingAnim);
        mushroomEvent.setThrowAnimationEvent.RemoveListener(SetThrowingAnim);
    }

    /// <summary>
    /// Set running animations based on data
    /// </summary>
    /// <param name="data">Movement data</param>
    private void RunningAnim(float movementData)
    {
        AnimatorClipInfo[] animInfo = playerAnim.GetCurrentAnimatorClipInfo(0);

        // Set float based on movement data
        playerAnim.SetFloat("Speed", Mathf.Abs((float)movementData));

        // Check movement data for which leg Anari is on for throwing animations
        if (playerAnim.GetFloat("Speed") > 0.01)
        {
            // If running, then check which leg the player is running on and update accordingly
            AnimatorClipInfo[] animationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
            AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            int currentFrame = (int)(animationClip[0].clip.length * (animationInfo.normalizedTime % 1) * animationClip[0].clip.frameRate);
            if (currentFrame == 50 || (currentFrame >= 0 && currentFrame < 25))
            {
                // Update for left foot
                playerAnim.SetBool("RunThrow_R", false);
            }
            else if (currentFrame >= 25 && currentFrame < 50)
            {
                // Update for right foot
                playerAnim.SetBool("RunThrow_R", true);
            }
        }
    }

    /// <summary>
    /// Set landing animations based on data
    /// </summary>
    /// <param name="data">Landing data</param>
    private void LandingAnim(bool landData)
    {
        // Set the landing bool based on data
        playerAnim.SetBool("Landed", landData);
    }

    /// <summary>
    /// Set jumping animatinos based on data
    /// </summary>
    /// <param name="data">Jumping data</param>
    private void JumpingAnim(bool jumpData)
    {
        // Set the jumping bool based on data
        playerAnim.SetBool("Jumping", jumpData);
    }

    /// <summary>
    /// Set falling animations based on data
    /// </summary>
    /// <param name="data">Falling data</param>
    private void FallingAnim(bool fallData)
    {
        // Set the falling bool based on data
        playerAnim.SetBool("Falling", fallData);
    }

    /// <summary>
    /// Set bouncing animations based on data
    /// </summary>
    /// <param name="data"></param>
    private void BouncingAnim(bool bounceData)
    {
        // Check if player is bouncing
        if(bounceData)
        {
            // If bouncing, set the trigger
            playerAnim.SetTrigger("Bouncing");
        } else
        {
            // If not bouncing, reset the trigger
            playerAnim.ResetTrigger("Bouncing");
        }
    }

    /// <summary>
    /// Check the throwing animation to see when to end
    /// </summary>
    private void CheckThrowingAnim()
    {
        if (playerAnim.GetBool("Throwing"))
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
    /// <param name="data">Throwing data</param>
    private void SetThrowingAnim(bool throwData)
    {
        playerAnim.SetBool("Throwing", throwData);
    }
}
