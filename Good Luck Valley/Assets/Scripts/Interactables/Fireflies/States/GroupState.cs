using GoodLuckValley.Architecture.StateMachine;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class GroupState : IState
    {
        protected readonly Fireflies controller;

        public GroupState(Fireflies controller)
        {
            this.controller = controller;
        }

        public virtual void OnEnter() { }
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void OnExit() { }
    }
}
