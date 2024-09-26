using GoodLuckValley.Patterns.StateMachine;

namespace GoodLuckValley.Entities.Fireflies.States
{
    public class FireflyState : IState
    {
        protected readonly Firefly firefly;

        public FireflyState(Firefly firefly)
        {
            this.firefly = firefly;
        }

        public virtual void FixedUpdate()
        {
            // Noop
        }

        public virtual void OnEnter()
        {
            // Noop
        }

        public virtual void OnExit()
        {
            // Noop
        }

        public virtual void Update()
        {
            // Noop
        }
    }
}
