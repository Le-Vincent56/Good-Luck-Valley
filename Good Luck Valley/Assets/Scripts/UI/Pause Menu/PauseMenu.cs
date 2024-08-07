using GoodLuckValley.Events;
using GoodLuckValley.Journal;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Control;
using GoodLuckValley.Player.Input;
using GoodLuckValley.SceneManagement;
using GoodLuckValley.UI.Menus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class PauseMenu : FadePanel
    {
        [Header("Events")]
        [SerializeField] private GameEvent onSetPauseInputAction;
        [SerializeField] private GameEvent onSetDefaultInputAction;
        [SerializeField] private GameEvent onOpenJournalMenu;
        [SerializeField] private GameEvent onOpenSettingsMenu;
        [SerializeField] private GameEvent onSetPauseBackground;

        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private PauseInputReader pauseInputReader;
        [SerializeField] private MenuCursor cursors;

        [Header("Fields")]
        [SerializeField] private float backTime;
        [SerializeField] private float backBuffer;

        protected override void Awake()
        {
            base.Awake();

            cursors = GetComponent<MenuCursor>();
        }

        private void OnEnable()
        {
            input.Pause += OnPause;
            pauseInputReader.Escape += OnPause;
            pauseInputReader.Navigate += NavigateUI;
            pauseInputReader.Submit += SubmitUI;
        }

        private void OnDisable()
        {
            input.Pause -= OnPause;
            pauseInputReader.Escape -= OnPause;
            pauseInputReader.Navigate -= NavigateUI;
            pauseInputReader.Submit -= SubmitUI;
        }

        public void Start()
        {
            // Hide the UI
            HideUIHard();
        }

        private void Update()
        {
            if(backTime > 0f)
            {
                backTime -= Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// Handle pause input
        /// </summary>
        /// <param name="started">If the button has been pressed</param>
        public void OnPause(bool started)
        {
            // Exit case - if the button hasn't been released
            if (started) return;

            // Exit case - the input buffer hasn't completed
            if (backTime > 0) return;

            // Set the input buffer
            backTime = backBuffer;

            // Toggle the pause
            TogglePause();
        }

        /// <summary>
        /// Handle pause events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnPause(Component sender, object data)
        {
            TogglePause();
        }

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
                cursors.DeactivateCursors();

                // Unpause the game
                PauseManager.Instance.UnpauseGame();

                // Set default input
                // Calls to:
                //  - PlayerInputHandler.EnableDefaultInput();
                onSetDefaultInputAction.Raise(this, null);

                // Remove the background
                // Calls to:
                // - PauseBackgroundFade.SetPauseBackground();
                onSetPauseBackground.Raise(this, false);

                // Hide the UI
                HideUI();
            } else
            {
                cursors.ShowCursors();
                cursors.ActivateCursors();

                // Pause the game
                PauseManager.Instance.PauseGame();

                // Set pause input
                // Calls to:
                //  - PlayerInputHandler.EnablePauseInput();
                onSetPauseInputAction.Raise(this, null);

                // Set the background
                // Calls to:
                // - PauseBackgroundFade.SetPauseBackground();
                onSetPauseBackground.Raise(this, true);

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
            // Exit case - the Journal has not been unlocked yet
            if (!Journal.Journal.Instance.Unlocked) return;

            // Hide the UI
            HideUI();

            // Open the Journal
            // Calls to:
            //  - JournalUI.OpenJournalMenu();
            onOpenJournalMenu.Raise(this, null);
        }

        public void OpenSettings()
        {
            // Deactivate cursors
            cursors.DeactivateCursors();

            // Hide UI
            HideUI();

            // Open the settings menu
            // Calls to:
            //  - GameSettingsMenu.OpenSettings();
            onOpenSettingsMenu.Raise(this, null);
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
            SceneLoader.Instance.LoadMainMenu();
        }

        /// <summary>
        /// Show the pause menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ShowPauseMenu(Component sender, object data)
        {
            // Set the pause input action
            // Calls to:
            //  - PlayerInputHandler.EnablePauseInput();
            onSetPauseInputAction.Raise(this, null);

            // Activate cursors
            cursors.ActivateCursors();

            // Show the UI
            ShowUI();
        }

        private void NavigateUI(Vector2 navigation)
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null)
            {
                // If nothing is selected, select the first selectable element
                Selectable firstSelectable = FindObjectOfType<Selectable>();
                if (firstSelectable != null)
                {
                    EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
                }
                return;
            }

            Selectable current = currentSelected.GetComponent<Selectable>();
            Selectable next = null;

            if (navigation.y > 0) next = current.FindSelectableOnUp();
            else if (navigation.y < 0) next = current.FindSelectableOnDown();
            else if (navigation.x < 0) next = current.FindSelectableOnLeft();
            else if (navigation.x > 0) next = current.FindSelectableOnRight();

            if (next != null)
            {
                EventSystem.current.SetSelectedGameObject(next.gameObject);
            }
        }

        private void SubmitUI(bool started)
        {
            // Exit case - if the button hasn't been released yet
            if (started) return;

            // Get the currently selected object
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            // Exit case - if there is not a currently selected object
            if (currentSelected == null) return;

            // Try to get a selectable
            Selectable selectable = currentSelected.GetComponent<Selectable>();

            // Exit case - if no selectable is retrieved
            if (selectable == null) return;

            // Check if the selectable is a button
            if (selectable is Button selectableButton)
            {
                // Cast and invoke
                selectableButton.onClick.Invoke();
            }
            // Check if the selectable is a toggle
            else if (selectable is Toggle selectableToggle)
            {
                // Cast and toggle
                selectableToggle.isOn = !selectableToggle.isOn;
            }
            // Check if the selectable is a dropdown
            else if (selectable is Dropdown dropdown)
            {
                // Show the dropdown
                dropdown.Show();
            }
        }
    }
}