using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.StartMenu.States
{
    public class DeleteIdleState : DeleteState
    {
        public DeleteIdleState(DeleteOverlay controller, Animator animator, CanvasGroup canvasGroup) 
            : base(controller, animator, canvasGroup)
        {
        }

        public override void OnEnter()
        {
            // Fade out the Canvas Group
            Fade(0f, fadeDuration, () =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });

            // Set the idle animation
            animator.CrossFade(IdleHash, crossFadeDuration);
        }
    }
}
