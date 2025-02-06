using GoodLuckValley.Architecture.StateMachine;

namespace GoodLuckValley.UI.Menus.Controls.States
{
    public class ControlsState : IState
    {
        protected readonly ControlsMainController controller;

        protected const float fadeTime = 0.2f;

        public ControlsState(ControlsMainController controller)
        {
            this.controller = controller;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }
}
