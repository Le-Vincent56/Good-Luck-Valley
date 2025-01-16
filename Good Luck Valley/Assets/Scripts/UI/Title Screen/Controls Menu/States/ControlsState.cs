using GoodLuckValley.Architecture.StateMachine;

namespace GoodLuckValley.UI.MainMenu.Controls.States
{
    public class ControlsState : IState
    {
        protected readonly ControlsMenuController controller;

        protected const float fadeTime = 0.2f;

        public ControlsState(ControlsMenuController controller)
        {
            this.controller = controller;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }
}
