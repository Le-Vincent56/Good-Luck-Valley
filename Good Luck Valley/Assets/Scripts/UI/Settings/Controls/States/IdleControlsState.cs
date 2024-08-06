using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Settings.Controls.States
{
    public class IdleControlsState : ControlsState
    {
        public IdleControlsState(GameControlsSettingController controller, List<Animator> animators) 
            : base(controller, animators)
        {
        }

        public override void OnEnter()
        {
            controller.DeactivateRebindingButtonAnimator();
        }

        public override void OnExit()
        {
            controller.ActivateRebindingButtonAnimator();
        }
    }
}