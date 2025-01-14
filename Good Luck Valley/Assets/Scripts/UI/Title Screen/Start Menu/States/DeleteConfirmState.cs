using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteConfirmState : DeleteState
    {
        private readonly Image loadingImage;
        private Tween imageFadeTween;

        public DeleteConfirmState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, Image loadingImage, Image contrastOverlay) 
            : base(controller, animator, canvasGroup, contrastOverlay)
        {
            this.loadingImage = loadingImage;
        }

        ~DeleteConfirmState()
        {
            // Kill any existing Tweens
            fadeGroupTween?.Kill();
            fadeOverlayTween?.Kill();
            imageFadeTween?.Kill();
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

            // Fade out the overlay
            FadeOverlay(0f, fadeDuration);

            // Set the slot data
            controller.SetSlotData();
        }

        /// <summary>
        /// Handle Fade Tweening for the Loading Iamge
        /// </summary>
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
