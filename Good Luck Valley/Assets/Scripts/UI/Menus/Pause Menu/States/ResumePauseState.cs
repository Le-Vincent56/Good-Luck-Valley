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
            Fade(0f, fadeDuration, Ease.InOutSine, () =>
            {
                screen.interactable = false;
                screen.blocksRaycasts = false;
            });

            // Set game controls
            controller.EnableGameInput();

            // Set the normal Time Scale
            Time.timeScale = 1f;

            // Set unpaused
            controller.Paused = false;

            // Hide the Pause Menu background
            controller.HideBackground();

            // Nullify the currently selected game object for the EventSystem
            EventSystem.current.SetSelectedGameObject(null);
        }

        public override void OnExit()
        {
            // Noop
        }
    }
}
