using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteState : IState
    {
        protected readonly DeleteOverlay controller;
        protected readonly Animator animator;
        protected readonly CanvasGroup canvasGroup;
        protected readonly Image contrastOverlay;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int DeletingHash = Animator.StringToHash("Deleting");

        protected const float crossFadeDuration = 0.1f;

        protected float fadeDuration;
        protected float overlayOpacity;
        protected Tween fadeGroupTween;
        protected Tween fadeOverlayTween;

        public DeleteState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, Image contrastOverlay)
        {
            this.controller = controller;
            this.animator = animator;
            this.canvasGroup = canvasGroup;
            this.contrastOverlay = contrastOverlay;
            overlayOpacity = 0.976f;
            fadeDuration = 0.2f;
        }

        ~DeleteState()
        {
            // Kill any existing Tweens
            fadeGroupTween?.Kill();
            fadeOverlayTween?.Kill();
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }

        /// <summary>
        /// Handle Fade Tweening for the Canvas Group
        /// </summary>
        protected void FadeGroup(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeGroupTween?.Kill();

            // Set the Fade Tween
            fadeGroupTween = canvasGroup.DOFade(endValue, duration);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeGroupTween.onComplete += onComplete;
        }

        /// <summary>
        /// Handle Fade Tweening for the Overlay Image
        /// </summary>
        protected void FadeOverlay(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeOverlayTween?.Kill();

            // Set the Fade Tween
            fadeOverlayTween = contrastOverlay.DOFade(endValue, duration);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeOverlayTween.onComplete += onComplete;
        }
    }
}
