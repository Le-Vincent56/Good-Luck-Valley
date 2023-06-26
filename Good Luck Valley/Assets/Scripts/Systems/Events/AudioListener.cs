using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListener : MonoBehaviour
{
    private EventInstance playerFootsteps;
    [SerializeField] private float stepTimerMax;
    [SerializeField] private float stepTimer;

    private void Start()
    {
        playerFootsteps = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootsteps);
        stepTimer = stepTimerMax;
    }

    private void OnEnable()
    {
        EventManager.StartListening("Footsteps", PlayFootsteps);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Footsteps", PlayFootsteps);
    }

    /// <summary>
    /// Play footsteps according to movement
    /// </summary>
    /// <param name="inputData">Player input data</param>
    /// <param name="movementData">Rigidbody data</param>
    /// <param name="groundedData">Grounded data</param>
    private void PlayFootsteps(object inputData, object movementData, object groundedData)
    {
        // Start footsteps event if the player has an x velocity and is on the ground
        if ((float)inputData != 0 && (bool)groundedData && (float)movementData != 0)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                // Play a footstep noise
                playerFootsteps.start();
            }
        }
    }
}
