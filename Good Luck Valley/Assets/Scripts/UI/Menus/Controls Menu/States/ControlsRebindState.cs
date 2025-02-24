using GoodLuckValley.UI.Menus.Controls;
using GoodLuckValley.UI.Menus.Controls.States;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley
{
    public class ControlsRebindState : ControlsState
    {
        protected List<Animator> animators;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BindingHash = Animator.StringToHash("Rebinding");

        protected const float crossFadeDuration = 0f;

        public ControlsRebindState(ControlsController controller, List<Animator> animators) 
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
