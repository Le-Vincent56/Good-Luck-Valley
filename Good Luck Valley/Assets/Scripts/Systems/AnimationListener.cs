using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationListener : MonoBehaviour
{
    Animator playerAnim;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        // Start listening to events
        EventManager.StartListening("Movement", RunningAnim);
        EventManager.StartListening("Landing", LandingAnim);
        EventManager.StartListening("Jumping", JumpingAnim);
        EventManager.StartListening("Falling", FallingAnim);
        EventManager.StartListening("Bouncing", BouncingAnim);
        EventManager.StartListening("CheckThrowing", CheckThrowingAnim);
        EventManager.StartListening("SetThrowing", SetThrowingAnim);
    }

    private void OnDisable()
    {
        // Stop listening to events
        EventManager.StopListening("Movement", RunningAnim);
        EventManager.StopListening("Landing", LandingAnim);
        EventManager.StopListening("Jumping", JumpingAnim);
        EventManager.StopListening("Falling", FallingAnim);
        EventManager.StopListening("Bouncing", BouncingAnim);
        EventManager.StopListening("CheckThrowing", CheckThrowingAnim);
        EventManager.StopListening("SetThrowing", SetThrowingAnim);
    }

    /// <summary>
    /// Set running animations based on data
    /// </summary>
    /// <param name="data">Movement data</param>
    private void RunningAnim(object data)
    {
        // Set float based on movement data
        playerAnim.SetFloat("Speed", Mathf.Abs((float)data));

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
    private void LandingAnim(object data)
    {
        // Set the landing bool based on data
        playerAnim.SetBool("Landed", (bool)data);
    }

    /// <summary>
    /// Set jumping animatinos based on data
    /// </summary>
    /// <param name="data">Jumping data</param>
    private void JumpingAnim(object data)
    {
        // Set the jumping bool based on data
        playerAnim.SetBool("Jumping", (bool)data);
    }

    /// <summary>
    /// Set falling animations based on data
    /// </summary>
    /// <param name="data">Falling data</param>
    private void FallingAnim(object data)
    {
        // Set the falling bool based on data
        playerAnim.SetBool("Falling", (bool)data);
    }

    /// <summary>
    /// Set bouncing animations based on data
    /// </summary>
    /// <param name="data"></param>
    private void BouncingAnim(object data)
    {
        // Check if player is bouncing
        bool bouncing = (bool)data;
        if(bouncing)
        {
            // If bouncing, set the trigger
            playerAnim.SetTrigger("Bouncing");
        } else
        {
            // If not bouncing, reset the trigger
            playerAnim.ResetTrigger("Bouncing");
        }
    }

    private void CheckThrowingAnim()
    {
        if (playerAnim.GetBool("Throwing"))
        {
            AnimatorClipInfo[] animationClip = playerAnim.GetCurrentAnimatorClipInfo(0);
            AnimatorStateInfo animationInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if (animationInfo.normalizedTime % 1 > 0.9)
            {
                EventManager.TriggerEvent("SetThrowing", false);
            }
        }
    }

    private void SetThrowingAnim(object data)
    {
        playerAnim.SetBool("Throwing", (bool)data);
    }
}
