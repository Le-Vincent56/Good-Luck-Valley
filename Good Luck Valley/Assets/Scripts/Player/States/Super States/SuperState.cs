using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public abstract class SuperState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        protected readonly ParticleController particles;
        public StateMachine subStates;

        public SuperState(PlayerController controller, AnimationController animator, ParticleController particles)
        {
            this.controller = controller;
            this.animator = animator;
            this.particles = particles;

            SetupSubStateMachine();
        }

        public virtual void OnEnter() { }

        public virtual void Update()
        {
            // Update the sub-State Machine
            subStates?.Update();
        }

        public virtual void FixedUpdate()
        {
            // Update the sub-State Machine
            subStates?.FixedUpdate();
        }

        public virtual void OnExit() { }

        public abstract void SetupSubStateMachine();
    }
}
