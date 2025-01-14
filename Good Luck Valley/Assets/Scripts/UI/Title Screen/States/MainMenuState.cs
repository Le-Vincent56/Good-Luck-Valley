using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class MainMenuState : IState
    {
        protected MainMenuController controller;
        protected CanvasGroup screen;
        protected IOptionMenu optionMenu;

        protected float fadeDuration;
        private Tween fadeTween;

        public MainMenuState(MainMenuController controller, CanvasGroup screen, IOptionMenu optionMenu)
        {
            this.controller = controller;
            this.screen = screen;
            this.optionMenu = optionMenu;
            fadeDuration = 0.5f;
        }

        public virtual void OnEnter()
        {
            Fade(1f, fadeDuration, Ease.InOutSine, () =>
            {
                screen.interactable = true;
                screen.blocksRaycasts = true;

                // Update the first selected of the Option Menu
                optionMenu.SelectFirst();
            });
        }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit()
        {
            Fade(0f, fadeDuration, Ease.InOutSine, () =>
            {
                screen.interactable = false;
                screen.blocksRaycasts = false;
            });

            // Update the first selected of the Option Menu
            optionMenu.UpdateFirst();
        }

        /// <summary>
        /// Handle Fade Tweening for the State's Canvas Group
        /// </summary>
        protected void Fade(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = screen.DOFade(endValue, duration);
            fadeTween.SetEase(easeType);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete = onComplete;
        }
    }
}
