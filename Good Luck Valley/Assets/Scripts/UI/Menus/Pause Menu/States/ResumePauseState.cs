using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Menus.Pause.States
{
    public class ResumePauseState : PauseMenuState
    {
        private bool initializing;

        public ResumePauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, optionMenu, fadeDuration)
        {
            initializing = true;
        }

        public override void OnEnter()
        {
            controller.Paused = false;

            // Check if the Pause Menu is not currently initializing
            if (!initializing)
            {
                // Hide the Pause Menu background
                controller.HideBackgroundExit(() =>
                {
                    // Set unpaused and resume time
                    Time.timeScale = 1f;

                    screen.interactable = false;
                    screen.blocksRaycasts = false;

                    EventBus<SetPaused>.Raise(new SetPaused() { Paused = false });
                });
            }
            // Set not initializing
            else initializing = false;

            // Nullify the currently selected game object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);

            // Hide the cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void OnExit()
        {
            // Disable all buttons
            optionMenu.DisableAllButtons();
        }
    }
}
