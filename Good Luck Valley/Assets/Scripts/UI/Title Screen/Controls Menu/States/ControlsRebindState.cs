using GoodLuckValley.UI.MainMenu.Controls;
using GoodLuckValley.UI.MainMenu.Controls.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class ControlsRebindState : ControlsState
    {
        protected Animator[] animators;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BindingHash = Animator.StringToHash("Rebinding");

        protected const float crossFadeDuration = 0f;

        public ControlsRebindState(ControlsMenuController controller, Animator[] animators) 
            : base(controller)
        {
            this.animators = animators;
        }

        public override void OnEnter()
        {
            // Cross fade to the rebinding animation
            animators[controller.CurrentRebindingButton].CrossFade(BindingHash, crossFadeDuration);
        }
    }
}
