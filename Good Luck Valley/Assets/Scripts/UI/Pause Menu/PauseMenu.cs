using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.UI
{
    public class PauseMenu : FadePanel
    {
        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onUpdatePaused;
        #endregion

        [Header("References")]
        [SerializeField] private InputReader input;

        [Header("Fields")]
        [SerializeField] private SoundData buttonSFX;

        #region PROPERTIES
        public bool Paused { get; private set; }
        public bool SoftPaused { get; private set; }
        #endregion

        private void OnEnable()
        {
            input.Pause += OnPause;
        }

        private void OnDisable()
        {
            input.Pause -= OnPause;
        }

        public void Start()
        {
            // Set paused to false initially
            Paused = false;

            // Set soft paused to false initially
            SoftPaused = false;

            // Hide the UI
            HideUIHard();
        }

        /// <summary>
        /// Handle pause input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        public void OnPause(bool started)
        {
            // If the button has been pressed, toggle pause
            if (started) TogglePause();
        }

        /// <summary>
        /// Toggle the pause menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void TogglePause()
        {
            // Don't pause if already soft paused
            if (SoftPaused) return;

            // Toggle paused
            Paused = !Paused;

            // Set effects
            if (Paused)
            {
                // Freeze time
                Time.timeScale = 0f;
                ShowUI();
            }
            else
            {
                // Resume time
                Time.timeScale = 1f;
                HideUI();
            }

            // Update paused for any listeners
            // Calls to:
            //  - PlayerInputHandler.SetPaused
            onUpdatePaused.Raise(this, Paused);
        }

        /// <summary>
        /// Toggle a soft pause
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ToggleSoftPause(Component sender, object data)
        {
            // Toggle pause
            SoftPaused = !SoftPaused;

            // Only freeze time
            if (SoftPaused)
            {
                // Freeze time
                Time.timeScale = 0f;
            }
            else
            {
                // Resume time
                Time.timeScale = 1f;
            }

            // Update paused for any listeners
            // Calls to:
            //  - PlayerInputHandler.SetPaused
            onUpdatePaused.Raise(this, SoftPaused);
        }

        /// <summary>
        /// Resume from the Pause Menu
        /// </summary>
        public void Resume()
        {
            // Since only called from the menu, TogglePause will unpause
            TogglePause();

            // Play the button sound effect
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Save from the Pause Menu
        /// </summary>
        public void Save()
        {
            // Save the game
            SaveLoadSystem.Instance.SaveGame();

            // Play the button sound effect
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Load the Main Menu
        /// </summary>
        public void ReturnToMain()
        {
            // Load the main menu scene
            SceneManager.LoadScene("Main Menu");

            // Play the button sound effect
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }
    }
}