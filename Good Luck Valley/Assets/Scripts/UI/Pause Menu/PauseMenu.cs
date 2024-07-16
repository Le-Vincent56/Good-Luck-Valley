using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.UI
{
    public class PauseMenu : FadePanel
    {
        [Header("Events")]
        [SerializeField] private GameEvent onSetPauseInputAction;
        [SerializeField] private GameEvent onSetDefaultInputAction;
        [SerializeField] private GameEvent onOpenJournalMenu;

        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private PauseInputReader pauseInputReader;

        private void OnEnable()
        {
            input.Pause += OnPause;
            pauseInputReader.Escape += OnPause;
        }

        private void OnDisable()
        {
            input.Pause -= OnPause;
            pauseInputReader.Escape -= OnPause;
        }

        public void Start()
        {
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
        /// Handle pause events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnPause(Component sender, object data) => TogglePause();

        /// <summary>
        /// Toggle the pause menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void TogglePause()
        {
            // Don't pause if already soft paused
            if (PauseManager.Instance.IsSoftPaused) return;

            // Toggle paused
            if(PauseManager.Instance.IsPaused)
            {
                // Unpause the game
                PauseManager.Instance.UnpauseGame();

                // Set default input
                // Calls to:
                //  - PlayerInputHandler.EnableDefaultInput();
                onSetDefaultInputAction.Raise(this, null);

                // Hide the UI
                HideUI();
            } else
            {
                // Pause the game
                PauseManager.Instance.PauseGame();

                // Set pause input
                // Calls to:
                //  - PlayerInputHandler.EnablePauseInput();
                onSetPauseInputAction.Raise(this, null);

                // Hide UI
                ShowUI();
            }
        }

        /// <summary>
        /// Toggle a soft pause (don't show UI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ToggleSoftPause(Component sender, object data)
        {
            // Toggle soft paused
            if(PauseManager.Instance.IsSoftPaused)
            {
                // Unpause the game
                PauseManager.Instance.SoftUnpauseGame();

                // Set default input
                // Calls to:
                //  - PlayerInputHandler.EnableDefaultInput();
                onSetDefaultInputAction.Raise(this, null);
            } else
            {
                // Pause the game
                PauseManager.Instance.SoftPauseGame();

                // Set pause input
                // Calls to:
                //  - PlayerInputHandler.EnablePauseInput();
                onSetPauseInputAction.Raise(this, null);
            }
        }

        /// <summary>
        /// Resume from the Pause Menu
        /// </summary>
        public void Resume()
        {
            // Since only called from the menu, TogglePause will unpause
            TogglePause();
        }

        /// <summary>
        /// Open the Journal from the Pause Menu
        /// </summary>
        public void OpenJournal()
        {
            // Open the Journal
            // Calls to:
            //  - JournalUI.OpenJournalMenu();
            onOpenJournalMenu.Raise(this, null);
        }

        /// <summary>
        /// Save from the Pause Menu
        /// </summary>
        public void Save()
        {
            // Save the game
            SaveLoadSystem.Instance.SaveGame();
        }

        /// <summary>
        /// Load the Main Menu
        /// </summary>
        public void ReturnToMain()
        {
            // Load the main menu scene
            SceneManager.LoadScene("Main Menu");
        }
    }
}