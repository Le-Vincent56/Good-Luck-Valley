using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Start.States
{
    public class IdleDeleteState : DeleteState
    {
        public IdleDeleteState(DeleteOverlayController controller, Animator animator, GameObject backgroundObj, GameObject animatedObj) 
            : base(controller, animator, backgroundObj, animatedObj)
        {
        }

        public override async void OnEnter()
        {
            // Check elements that need to be hidden
            if (!CheckHidden(0) && !CheckHidden(1))
                await Hide(fadeTime);
            else if (CheckHidden(0) && !CheckHidden(1))
                await HideAnimation(fadeTime);
            else if (!CheckHidden(0) && CheckHidden(1))
                await HideBackground(fadeTime);

            animator.CrossFade(IdleHash, crossFadeDuration);
        }

        /// <summary>
        /// Check if certain elements are hidden
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool CheckHidden(int count)
        {
            if (images[count].color.a != 0) return false;

            return true;
        }
    }
}