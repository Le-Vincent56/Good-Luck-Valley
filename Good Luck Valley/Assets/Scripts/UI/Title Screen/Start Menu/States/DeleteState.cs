using DG.Tweening;
using GoodLuckValley.Architecture.StateMachine;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteState : IState
    {
        protected readonly DeleteOverlay controller;
        protected readonly Animator animator;
        protected readonly CanvasGroup canvasGroup;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int DeletingHash = Animator.StringToHash("Deleting");

        protected const float crossFadeDuration = 0.1f;

        protected float fadeDuration;
        private Tween fadeTween;

        public DeleteState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup)
        {
            this.controller = controller;
            this.animator = animator;
            this.canvasGroup = canvasGroup;
            fadeDuration = 0.2f;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }

        protected void Fade(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, duration);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete += onComplete;
        }
    }
}
