using UnityEngine;


namespace GoodLuckValley.UI.TitleScreen.Settings.Controls.States
{
    public class BindingControlsState : ControlsState
    {
        public BindingControlsState(ControlsSettingController controller, GameObject panel) 
            : base(controller, panel)
        {
        }

        public override async void OnEnter()
        {
            animator.CrossFade(BindingHash, crossFadeDuration);

            await Show();
        }

        public override async void OnExit()
        {
            await Hide();
        }
    }
}