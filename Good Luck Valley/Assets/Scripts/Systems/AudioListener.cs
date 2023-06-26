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
        EventManager.StartListening("Footsteps", UpdateSound);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Footsteps", UpdateSound);
    }

    private void UpdateSound(object inputData, object movementData, object groundedData)
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
