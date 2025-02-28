using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Menus.Pause.States
{
    public class ResumePauseState : PauseMenuState
    {
        public ResumePauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            controller.Paused = false;

            // Hide the Pause Menu background
            controller.HideBackgroundExit(() =>
            {
                // Set unpaused and resume time
                
                Time.timeScale = 1f;

                screen.interactable = false;
                screen.blocksRaycasts = false;
            });

            // Nullify the currently selected game object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);

            Debug.Log("Entered Resume State");
        }

        public override void OnExit()
        {
            // Noop
        }
    }
}
