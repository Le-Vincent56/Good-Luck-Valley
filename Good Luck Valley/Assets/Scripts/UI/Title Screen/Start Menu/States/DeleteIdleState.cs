using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteIdleState : DeleteState
    {
        public DeleteIdleState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup, Image contrastOverlay) 
            : base(controller, animator, canvasGroup, contrastOverlay)
        {
        }

        public override void OnEnter()
        {
            // Fade out the Canvas Group
            FadeGroup(0f, fadeDuration, () =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });

            // Set the idle animation
            animator.CrossFade(IdleHash, crossFadeDuration);
        }
    }
}
