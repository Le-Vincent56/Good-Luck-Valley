using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls.States
{
    public class IdleControlsState : ControlsState
    {
        public IdleControlsState(ControlsSettingController controller, GameObject panel) 
            : base(controller, panel)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);

            MakeElementsInvisible();
        }
    }
}