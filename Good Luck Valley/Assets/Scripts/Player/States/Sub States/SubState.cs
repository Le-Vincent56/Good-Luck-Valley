using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class SubState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        protected readonly PlayerParticleController particles;
        protected readonly PlayerSFX sfx;
        public StateMachine subStates;

        public SubState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
        {
            this.controller = controller;
            this.animator = animator;
            this.particles = particles;
        }

        public SubState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
            : this(controller, animator, particles)
        {
            this.controller = controller;
            this.animator = animator;
            this.particles = particles;
            this.sfx = sfx;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }
}
