using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioListener : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    private EventInstance playerFootstepsGrass;
    private EventInstance playerFootstepsDirt;
    private EventInstance bushRustles;
    [SerializeField] private bool usePlayerSFX = true;
    [SerializeField] private float stepTimerMax;
    [SerializeField] private float stepTimer;
    [SerializeField] private bool playingFootstepsDuringCutscene = false;
    #endregion

    private void OnEnable()
    {
        //EventManager.StartListening("Footsteps", PlayFootsteps);
        movementEvent.footstepEvent.AddListener(PlayFootstepsRun);
        movementEvent.startFootstepEventCutscene.AddListener(PlayFootstepsCutscene);
        movementEvent.stopFootstepEventCutscene.AddListener(StopFootstepsCutscene);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("Footsteps", PlayFootsteps);
        movementEvent.footstepEvent.RemoveListener(PlayFootstepsRun);
        movementEvent.startFootstepEventCutscene.RemoveListener(PlayFootstepsCutscene);
        movementEvent.stopFootstepEventCutscene.RemoveListener(StopFootstepsCutscene);
    }

    private void Start()
    {
        playerFootstepsGrass = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootstepsGrass);
        playerFootstepsDirt = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootstepsDirt);
        stepTimer = stepTimerMax;
    }

    /// <summary>
    /// Play footsteps according to movement
    /// </summary>
    /// <param name="inputData">Player input data</param>
    /// <param name="movementData">Rigidbody data</param>
    /// <param name="groundedData">Grounded data</param>
    /// <param name="tileType">The type of tile the player is on</param>
    private void PlayFootstepsRun(float inputData, float movementData, bool groundedData, TileType tileType)
    {
        // Start footsteps event if the player has an x velocity and is on the ground
        if (inputData != 0 && groundedData && movementData != 0)
        {
            // Choose footsteps based on tile type
            ChooseFootsteps(tileType);
        }
    }

    /// <summary>
    /// Play footsteps during a cutscene according to movement
    /// </summary>
    /// <param name="movementData">Rigidbody data</param>
    /// <param name="groundedData">Grounded data</param>
    /// <param name="tileType">The type of tile the player is on</param>
    private void PlayFootstepsCutscene(float movementData, bool groundedData, TileType tileType)
    {
        if(!playingFootstepsDuringCutscene)
        {
            StartCoroutine(PlayUntilLoad(movementData, groundedData, tileType));
        }
        
    }

    private void StopFootstepsCutscene(float movementData, bool groundedData, TileType tileType)
    {
        if(playingFootstepsDuringCutscene)
        {
            // Check if the grass footstep is playing
            PLAYBACK_STATE playbackStatePrevGrass;
            playerFootstepsGrass.getPlaybackState(out playbackStatePrevGrass);
            if (!playbackStatePrevGrass.Equals(PLAYBACK_STATE.STOPPED))
            {
                // If so, stop it to prevent overlap
                playerFootstepsGrass.stop(STOP_MODE.IMMEDIATE);
            }

            // Check if the grass footstep is playing
            PLAYBACK_STATE playbackStateDirt;
            playerFootstepsGrass.getPlaybackState(out playbackStateDirt);
            if (!playbackStateDirt.Equals(PLAYBACK_STATE.STOPPED))
            {
                // If so, stop it to prevent overlap
                playerFootstepsGrass.stop(STOP_MODE.IMMEDIATE);
            }

            StopCoroutine(PlayUntilLoad(movementData, groundedData, tileType));
            playingFootstepsDuringCutscene = false;
        }
    }

    /// <summary>
    /// Choose footsteps depending on the tile type
    /// </summary>
    /// <param name="tileType">The type of tile the player is on</param>
    public void ChooseFootsteps(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Dirt:
                // Check if the grass footstep is playing
                PLAYBACK_STATE playbackStatePrevDirt;
                playerFootstepsGrass.getPlaybackState(out playbackStatePrevDirt);
                if (!playbackStatePrevDirt.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, stop it to prevent overlap
                    playerFootstepsGrass.stop(STOP_MODE.IMMEDIATE);
                }

                // Check if the dirt footstep has stopped
                PLAYBACK_STATE playbackStateDirt;
                playerFootstepsDirt.getPlaybackState(out playbackStateDirt);
                if (playbackStateDirt.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, start it
                    playerFootstepsDirt.start();
                }
                break;

            case TileType.Grass:
                // Check if the dirt footstep is playing
                PLAYBACK_STATE playbackStatePrevGrass;
                playerFootstepsDirt.getPlaybackState(out playbackStatePrevGrass);
                if (!playbackStatePrevGrass.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, stop it to prevent overlap
                    playerFootstepsDirt.stop(STOP_MODE.IMMEDIATE);
                }

                // Check if the grass footstep has stopped
                PLAYBACK_STATE playbackStateGrass;
                playerFootstepsGrass.getPlaybackState(out playbackStateGrass);
                if (playbackStateGrass.Equals(PLAYBACK_STATE.STOPPED))
                {
                    // If so, start it
                    playerFootstepsGrass.start();
                }
                break;

            case TileType.None:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Play footsteps until the end of a cutscene
    /// </summary>
    /// <param name="movementData">Rigidbody data</param>
    /// <param name="groundedData">Grounded data</param>
    /// <param name="tileType">The type of tile the player is on</param>
    /// <returns></returns>
    private IEnumerator PlayUntilLoad(float movementData, bool groundedData, TileType tileType)
    {
        playingFootstepsDuringCutscene = true;

        while(cutsceneEvent.GetPlayingCutscene())
        {
            yield return null;

            if (movementData != 0 && groundedData)
            {
                ChooseFootsteps(tileType);
            }
        }
    }
}
