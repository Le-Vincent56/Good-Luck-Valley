using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListener : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private MovementScriptableObj movementEvent;
    private EventInstance playerFootsteps;
    [SerializeField] private bool usePlayerSFX = true;
    [SerializeField] private float stepTimerMax;
    [SerializeField] private float stepTimer;
    #endregion

    public EventInstance PlayerFootsteps { get { return playerFootsteps; } }

    private void OnEnable()
    {
        //EventManager.StartListening("Footsteps", PlayFootsteps);
        movementEvent.footstepEvent.AddListener(PlayFootsteps);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("Footsteps", PlayFootsteps);
        movementEvent.footstepEvent.RemoveListener(PlayFootsteps);
    }

    private void OnSceneLoaded()
    {
        if ((float)AudioManager.Instance.CurrentArea == 0)
        {
            usePlayerSFX = false;
        }
        else
        {
            usePlayerSFX = true;
        }
    }

    private void Start()
    {
        if(usePlayerSFX)
        {
            playerFootsteps = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootsteps);
        }
        stepTimer = stepTimerMax;
    }

    /// <summary>
    /// Play footsteps according to movement
    /// </summary>
    /// <param name="inputData">Player input data</param>
    /// <param name="movementData">Rigidbody data</param>
    /// <param name="groundedData">Grounded data</param>
    private void PlayFootsteps(float inputData, float movementData, bool groundedData)
    {
        // Start footsteps event if the player has an x velocity and is on the ground
        if (inputData != 0 && groundedData && movementData != 0)
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
