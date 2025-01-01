using GoodLuckValley.Architecture.StateMachine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyState : IState
    {
        protected readonly Firefly firefly;

        public FireflyState(Firefly firefly)
        {
            this.firefly = firefly;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }
}
