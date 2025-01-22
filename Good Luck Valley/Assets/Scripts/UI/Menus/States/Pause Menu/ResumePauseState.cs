using DG.Tweening;
using GoodLuckValley.UI.Menus.OptionMenus;
using GoodLuckValley.UI.Menus.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class ResumePauseState : PauseMenuState
    {
        public ResumePauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            // Set game controls
            controller.EnableGameInput();

            // Set the normal Time Scale
            Time.timeScale = 1f;

            // Set unpaused
            controller.Paused = false;
        }

        public override void OnExit()
        {
            // Noop
        }
    }
}
