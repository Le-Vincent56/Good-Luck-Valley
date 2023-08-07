using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HiveMind.Events;
using HiveMind.Particles;

namespace HiveMind.Audio
{
    public class AudioListener : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private MovementScriptableObj movementEvent;
        [SerializeField] private CutsceneScriptableObj cutsceneEvent;
        [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
        private EventInstance playerFootstepsGrass;
        private EventInstance playerFootstepsDirt;
        private EventInstance playerFall;
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
            movementEvent.fallEvent.AddListener(PlayFall);
            movementEvent.landEvent.AddListener(StopFall);
            loadLevelEvent.startForestMusicLoad.AddListener(UpdateForestMusicProgress);
        }

        private void OnDisable()
        {
            //EventManager.StopListening("Footsteps", PlayFootsteps);
            movementEvent.footstepEvent.RemoveListener(PlayFootstepsRun);
            movementEvent.startFootstepEventCutscene.RemoveListener(PlayFootstepsCutscene);
            movementEvent.stopFootstepEventCutscene.RemoveListener(StopFootstepsCutscene);
            movementEvent.fallEvent.RemoveListener(PlayFall);
            movementEvent.landEvent.RemoveListener(StopFall);
            loadLevelEvent.startForestMusicLoad.RemoveListener(UpdateForestMusicProgress);
        }

        private void Start()
        {
            playerFootstepsGrass = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootstepsGrass);
            playerFootstepsDirt = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFootstepsDirt);
            playerFall = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PlayerFall);
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
            if (!playingFootstepsDuringCutscene)
            {
                StartCoroutine(PlayUntilLoad(movementData, groundedData, tileType));
            }
        }

        /// <summary>
        /// Stop cutscene footsteps according to movement
        /// </summary>
        /// <param name="movementData">Rigidbody data</param>
        /// <param name="groundedData">Grounded data</param>
        /// <param name="tileType">The type of tile the player is on</param>
        private void StopFootstepsCutscene(float movementData, bool groundedData, TileType tileType)
        {
            if (playingFootstepsDuringCutscene)
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
        /// Play fall sound
        /// </summary>
        private void PlayFall()
        {
            // Check if the player fall has stopped or is already playing
            PLAYBACK_STATE playbackStateFall;
            playerFall.getPlaybackState(out playbackStateFall);
            if (playbackStateFall.Equals(PLAYBACK_STATE.STOPPED) && !playbackStateFall.Equals(PLAYBACK_STATE.PLAYING))
            {
                // If so, start it
                playerFall.start();
            }
        }

        /// <summary>
        /// Stop the fall sound
        /// </summary>
        private void StopFall()
        {
            // Check if the player fall is playing
            PLAYBACK_STATE playbackStateFall;
            playerFall.getPlaybackState(out playbackStateFall);
            if (!playbackStateFall.Equals(PLAYBACK_STATE.STOPPED))
            {
                // If so, stop it
                playerFall.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        /// <summary>
        /// Update Forest Progress to 0
        /// </summary>
        /// <param name="progressLevel">The level to progress to</param>
        public void UpdateForestMusicProgress(ForestLayer forestLayer)
        {
            StartCoroutine(UpdateForestMusicProgressEnum(forestLayer));
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

            while (cutsceneEvent.GetPlayingCutscene())
            {
                yield return null;

                if (movementData != 0 && groundedData)
                {
                    ChooseFootsteps(tileType);
                }
            }
        }

        /// <summary>
        /// Update Forest Progress
        /// </summary>
        /// <param name="progressLevel">The level to progress to</param>
        /// <returns></returns>
        private IEnumerator UpdateForestMusicProgressEnum(ForestLayer forestLayer)
        {
            if (AudioManager.Instance.CurrentForestLevel == ForestLevel.MAIN)
            {
                // Create a list to store keys to be modified
                List<string> keysToModify = new List<string>();
                foreach (KeyValuePair<string, float> layer in AudioManager.Instance.ForestLayers)
                {
                    if (layer.Key != forestLayer.ToString())
                    {
                        keysToModify.Add(layer.Key);
                    }
                }

                // Loop until layer is in fully
                while (AudioManager.Instance.ForestLayers[forestLayer.ToString()] <= 1)
                {
                    // Fade-in the intended layer
                    AudioManager.Instance.SetForestLayer(forestLayer.ToString(), AudioManager.Instance.ForestLayers[forestLayer.ToString()] + (Time.deltaTime / 4f));

                    // Modify the dictionary outside the loop
                    foreach (string key in keysToModify)
                    {
                        // Fade-out all other layers
                        if (key != forestLayer.ToString() && AudioManager.Instance.ForestLayers[key] >= 0)
                        {
                            AudioManager.Instance.SetForestLayer(key, AudioManager.Instance.ForestLayers[key] - (Time.deltaTime / 4f));
                        }
                    }
                    yield return null;
                }

                // Round them to a perfect integer for volume clarity
                foreach (string key in keysToModify)
                {
                    AudioManager.Instance.SetForestLayer(key, Mathf.Round(AudioManager.Instance.ForestLayers[key]));
                }

                yield return null;
            }
        }
    }
}
