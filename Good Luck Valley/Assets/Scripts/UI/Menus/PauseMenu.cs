using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using HiveMind.Audio;
using HiveMind.Events;
using HiveMind.SaveData;

namespace HiveMind.Menus
{
    public class PauseMenu : MonoBehaviour, IData
    {
        #region REFERENCES
        [SerializeField] PauseScriptableObj pauseEvent;
        [SerializeField] JournalScriptableObj journalEvent;
        [SerializeField] DisableScriptableObj disableEvent;
        [SerializeField] SaveMenuScriptableObj saveMenuEvent;
        private Canvas pauseUI;
        private Canvas settingsUI;
        #endregion

        #region FIELDS
        [SerializeField] private bool paused = false;
        private string levelName;
        private float playtimeTotal;
        private float playtimeHours;
        private float playtimeMinutes;
        private float playtimeSeconds;
        private string playtimeString;
        #endregion

        #region PROPERTIES
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            pauseUI = GameObject.Find("PauseUI").GetComponent<Canvas>();
            pauseUI.enabled = false;
            settingsUI = GameObject.Find("SettingsUI").GetComponent<Canvas>();
            settingsUI.enabled = false;

            levelName = SceneManager.GetActiveScene().name;

            // Start time record coroutine
            StartCoroutine(RecordTimeRoutine());
        }

        /// <summary>
        /// Toggle the pause menu
        /// </summary>
        /// <param name="context">The context of the controller</param>
        public void TogglePause(InputAction.CallbackContext context)
        {
            if ((!journalEvent.GetJournalOpen() && journalEvent.GetCloseBuffer() <= 0) && (!saveMenuEvent.GetSaveMenuOpen() && saveMenuEvent.GetSaveCloseBuffer() <= 0) && pauseEvent.GetCanPause() && !settingsUI.enabled)
            {
                if (!paused)
                {
                    paused = true;
                    pauseUI.enabled = true;
                    Time.timeScale = 0;
                    pauseEvent.Pause();
                    pauseEvent.SetPauseMenuOpen(true);
                }
                else
                {
                    paused = false;
                    pauseUI.enabled = false;
                    Time.timeScale = 1f;
                    pauseEvent.Unpause();
                    pauseEvent.SetPauseMenuOpen(false);
                }
            }
        }

        /// <summary>
        /// Continue the Game
        /// </summary>
        public void Continue()
        {
            if (!journalEvent.GetJournalOpen() && !saveMenuEvent.GetSaveMenuOpen() && !settingsUI.enabled)
            {
                paused = false;
                pauseUI.enabled = false;
                Time.timeScale = 1f;
                pauseEvent.Unpause();
                pauseEvent.SetPauseMenuOpen(false);

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
            }
        }

        /// <summary>
        /// Take the Player to Settings screen
        /// </summary>
        /// <param name="scene">The scene number that represents the Settings scene</param>
        public void Settings()
        {
            if (!journalEvent.GetJournalOpen() && !saveMenuEvent.GetSaveMenuOpen() && !settingsUI.enabled)
            {
                paused = true;
                pauseUI.enabled = false;
                Time.timeScale = 0f;
                settingsUI.enabled = true;

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
            }
        }

        /// <summary>
        /// Closes the in-game settings menu
        /// </summary>
        public void CloseSettings()
        {
            pauseUI.enabled = true;
            settingsUI.enabled = false;
            paused = true;

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }

        /// <summary>
        /// Save the Game
        /// </summary>
        public void Save()
        {
            // Disable pause UI
            pauseUI.enabled = false;

            // Activate the save menu
            saveMenuEvent.ActivateSaveMenu();

            // Play sound
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
        }

        /// <summary>
        /// Quit to Title
        /// </summary>
        /// <param name="scene">Scene number that represents Quitting to Title</param>
        public void Quit(int scene)
        {
            if (!journalEvent.GetJournalOpen() && !saveMenuEvent.GetSaveMenuOpen())
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(scene);
                pauseEvent.SetPauseMenuOpen(false);

                // Play sound
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIButton, transform.position);
            }
        }

        /// <summary>
        /// Coroutine to record playtime
        /// </summary>
        /// <returns>The total time played stored in variables</returns>
        public IEnumerator RecordTimeRoutine()
        {
            while (!paused)
            {
                // Record playtime every second
                yield return new WaitForSeconds(1);
                playtimeTotal += 1;
            }
        }

        #region DATA HANDLING
        public void LoadData(GameData data)
        {
            levelName = data.currentLevelName;
            playtimeTotal = data.playtimeTotal;
            playtimeString = data.playtimeString;
        }

        public void SaveData(GameData data)
        {
            #region CALCULATE PLAYTIME
            // Turn playtime into hours, minutes, and seconds
            playtimeHours = (int)(playtimeTotal / 3600) % 24;
            playtimeMinutes = (int)(playtimeTotal / 60) % 60;
            playtimeSeconds = (int)playtimeTotal % 60;

            // Create a playtime string
            playtimeString = playtimeHours + ":" + playtimeMinutes + ":" + playtimeSeconds;
            #endregion

            data.currentLevelName = levelName;
            data.playtimeTotal = playtimeTotal;
            data.playtimeString = playtimeString;
        }
        #endregion
    }
}
