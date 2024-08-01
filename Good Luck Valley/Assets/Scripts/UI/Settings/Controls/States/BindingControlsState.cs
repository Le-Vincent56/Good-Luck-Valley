using System.Collections.Generic;
using UnityEngine;


namespace GoodLuckValley.UI.Settings.Controls.States
{
    public class BindingControlsState : ControlsState
    {
        public BindingControlsState(GameControlsSettingController controller, List<Animator> animators) 
            : base(controller, animators)
        {
        }

        public override void OnEnter()
        {
            animators[controller.CurrentRebindingButton].CrossFade(BindingHash, crossFadeDuration);
        }
    }
}