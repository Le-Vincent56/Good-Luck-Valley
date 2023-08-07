using HiveMind.Events;
using HiveMind.SaveData;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace HiveMind.LevelLoad
{
    public enum LEVELPOS
    {
        DEFAULT,
        ENTER,
        RETURN
    }

    public class LoadLevel : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private MovementScriptableObj movementEvent;
        [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
        [SerializeField] private CutsceneScriptableObj cutsceneEvent;
        [SerializeField] private PauseScriptableObj pauseEvent;
        [SerializeField] private LevelDataObj levelDataObj;
        [SerializeField] private DisableScriptableObj disableEvent;
        private GameObject player;
        public Animator transition;
        #endregion

        #region FIELDS
        [SerializeField] private bool useLevelData = false;
        public float transitionTime = 1f;
        #endregion

        #region PROPERTIES
        public bool UseLevelData { get { return useLevelData; } set { useLevelData = value; } }
        #endregion

        /// <summary>
        /// Load the next level
        /// </summary>
        public void LoadNextLevel()
        {
            StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

        /// <summary>
        /// Load the previous level
        /// </summary>
        public void LoadPrevLevel()
        {
            // Save the level before loading
            DataManager.Instance.SaveGame();

            StartCoroutine(LoadingLevel(SceneManager.GetActiveScene().buildIndex - 1));
        }

        /// <summary>
        /// Pauses before triggering level change animation
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <returns></returns>
        IEnumerator LoadingLevel(int levelIndex)
        {
            // Play aimation
            transition.SetTrigger("Start");

            // Wait
            yield return new WaitForSeconds(transitionTime);

            // De-activate triggers
            loadLevelEvent.SetTriggersActive(false);

            // Load scene
            SceneManager.LoadScene(levelIndex);
        }

        /// <summary>
        /// Start loading a level
        /// </summary>
        public void StartLoading()
        {
            // Disable the HUD while loading
            disableEvent.DisableHUD();

            // Prevent the player from pausing
            pauseEvent.SetCanPause(false);
            pauseEvent.SetPauseMenuOpen(false);
            pauseEvent.SetPaused(false);

            // Play the respective cutscenes depending on whether the player is entering, returning, or just loading in
            switch (levelDataObj.levelPosData[SceneManager.GetActiveScene().name].levelPos)
            {
                case LEVELPOS.ENTER:
                    // Set the enter cutscene
                    cutsceneEvent.SetEnterCutscene(levelDataObj.levelPosData[SceneManager.GetActiveScene().name].enterCutscene);

                    // Stop any footstep noises
                    movementEvent.StopCutsceneFootstepEvent();

                    // Start the cutscene
                    cutsceneEvent.StartEnterCutscene();

                    // Start cutscene footstep noises
                    movementEvent.StartCutsceneFootstepEvent();
                    break;

                case LEVELPOS.RETURN:
                    // Set the return cutscene
                    cutsceneEvent.SetEnterCutscene(levelDataObj.levelPosData[SceneManager.GetActiveScene().name].returnCutscene);

                    // Stop any footstep noises
                    movementEvent.StopCutsceneFootstepEvent();

                    // Start the cutscene
                    cutsceneEvent.StartEnterCutscene();

                    // Start cutscene footstep noises
                    movementEvent.StartCutsceneFootstepEvent();
                    break;

                case LEVELPOS.DEFAULT:
                    break;
            }

            // Reset load triggers
            loadLevelEvent.SetInLoadTrigger(false);

            // Trigger start load event
            loadLevelEvent.StartLoad();
        }

        /// <summary>
        /// End loading a level
        /// </summary>
        public void EndLoading()
        {
            // If loading through a cutscene, return so there's no overlap
            if (loadLevelEvent.GetLoadingThroughCutscene())
            {
                loadLevelEvent.SetLoadingThroughCutscene(false);
                return;
            }

            // Trigger end load event
            loadLevelEvent.EndLoad();

            // Save the game
            DataManager.Instance.SaveGame();

            // Reset movement obj
            movementEvent.ResetObj();

            // Check to see if the lotus cutscene needs to be played
            PlayableDirector camDirector = GameObject.Find("Main Camera").GetComponent<PlayableDirector>();
            if (camDirector != null)
            {
                if (camDirector.enabled)
                {
                    cutsceneEvent.StartLotusCutscene();
                }
                else
                {
                    // Enable the HUD is a cutscene doesn't start
                    disableEvent.EnableHUD();
                }
            }
            else
            {
                // Enable the HUD if a director doesn't exist
                disableEvent.EnableHUD();
            }

            // Set load triggers to be active
            loadLevelEvent.SetTriggersActive(true);

            // Allow the player to pause
            pauseEvent.SetCanPause(true);
        }

        /// <summary>
        /// End loading for a level through a signal receiver - mainly for entering/leaving cutscenes
        /// </summary>
        public void EndLoadingAfterCutscene()
        {
            // Trigger end load event
            loadLevelEvent.EndLoad();

            // Save the game
            DataManager.Instance.SaveGame();

            // Reset movement obj
            movementEvent.ResetObj();

            // Set the movement to 0 so there's no animation bugs
            movementEvent.SetMovementDirection(new Vector2(0, movementEvent.GetMovementVector().y));
            movementEvent.ApplyMovementDirection();

            // Check to see if the lotus cutscene needs to be played
            PlayableDirector camDirector = GameObject.Find("Main Camera").GetComponent<PlayableDirector>();
            if (camDirector != null)
            {
                if (camDirector.enabled)
                {
                    cutsceneEvent.StartLotusCutscene();
                }
                else
                {
                    // Enable the HUD is a cutscene doesn't start
                    disableEvent.EnableHUD();
                }
            }
            else
            {
                // Enable the HUD if a director doesn't exist
                disableEvent.EnableHUD();
            }

            // End the enter cutscene
            cutsceneEvent.EndEnterCutscene();

            // Stop the cutscene footstep event
            movementEvent.StopCutsceneFootstepEvent();

            // Set to false in case of future problems
            loadLevelEvent.SetLoadingThroughCutscene(false);

            // Set load triggers to be active
            loadLevelEvent.SetTriggersActive(true);

            // Allow the player to pause
            pauseEvent.SetCanPause(true);
        }
    }
}
