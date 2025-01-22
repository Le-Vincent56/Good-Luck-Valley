namespace GoodLuckValley.UI.Menus.Controls.States
{
    public class ControlsIdleState : ControlsState
    {
        public ControlsIdleState(ControlsMenuController controller)
            : base(controller)
        {
        }

        public override void OnEnter()
        {
            // Deactivate the Rebinding Button Animator
            controller.DeactivateRebindingButtonAnimator();
        }

        public override void OnExit()
        {
            // Activate the Rebinding Button Animator
            controller.ActivateRebindingButtonAnimator();
        }
    }
}
