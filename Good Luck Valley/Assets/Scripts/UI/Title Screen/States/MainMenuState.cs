using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.UI.MainMenu.OptionMenus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.States
{
    public class MainMenuState : IState
    {
        protected MainMenuController controller;
        protected CanvasGroup screen;
        protected IOptionMenu optionMenu;
        protected Image darkerBackground;

        protected float fadeDuration;
        private Tween fadeGroupTween;
        private Tween fadeBackgroundTween;

        public MainMenuState(MainMenuController controller, CanvasGroup screen, Image darkerBackground, IOptionMenu optionMenu, float fadeDuration)
        {
            this.controller = controller;
            this.screen = screen;
            this.darkerBackground = darkerBackground;
            this.optionMenu = optionMenu;
            this.fadeDuration = fadeDuration;
        }

        ~MainMenuState()
        {
            // Kill the Fade Tweens
            fadeGroupTween?.Kill();
            fadeBackgroundTween?.Kill();
        }

        public virtual void OnEnter()
        {
            FadeGroup(1f, fadeDuration, Ease.InOutSine, () =>
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
            FadeGroup(0f, fadeDuration, Ease.InOutSine, () =>
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
        protected void FadeGroup(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeGroupTween?.Kill();

            // Set the Fade Tween
            fadeGroupTween = screen.DOFade(endValue, duration);
            fadeGroupTween.SetEase(easeType);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeGroupTween.onComplete = onComplete;
        }

        protected void FadeBackground(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeBackgroundTween?.Kill();

            // Set the Fade Tween
            fadeBackgroundTween = darkerBackground.DOFade(endValue, duration);
            fadeBackgroundTween.SetEase(easeType);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeBackgroundTween.onComplete = onComplete;
        }
    }
}
