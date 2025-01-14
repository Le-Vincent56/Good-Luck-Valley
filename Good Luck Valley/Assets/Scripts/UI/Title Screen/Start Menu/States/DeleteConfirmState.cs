using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteConfirmState : DeleteState
    {
        private Image loadingImage;
        private Tween imageFadeTween;

        public DeleteConfirmState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, Image loadingImage) : base(controller, animator, canvasGroup)
        {
            this.loadingImage = loadingImage;
        }

        public override void OnEnter()
        {
            // Fade in the Loading Image
            FadeImage(1f, fadeDuration);

            // Set the loading animation
            animator.CrossFade(DeletingHash, crossFadeDuration);
        }

        public override void OnExit()
        {
            // Fade out the Loading Image
            FadeImage(0f, fadeDuration);
        }

        private void FadeImage(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            imageFadeTween?.Kill();

            // Set the Fade Tween
            imageFadeTween = loadingImage.DOFade(endValue, duration);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            imageFadeTween.onComplete += onComplete;
        }
    }
}
