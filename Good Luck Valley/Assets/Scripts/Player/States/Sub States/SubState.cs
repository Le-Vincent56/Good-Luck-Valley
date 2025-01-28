using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class SubState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        protected readonly ParticleController particles;
        public StateMachine subStates;

        public SubState(PlayerController controller, AnimationController animator, ParticleController particles)
        {
            this.controller = controller;
            this.animator = animator;
            this.particles = particles;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }
}
