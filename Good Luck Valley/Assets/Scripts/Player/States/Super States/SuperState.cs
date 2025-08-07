using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public abstract class SuperState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        protected readonly PlayerParticleController particles;
        protected readonly PlayerSFX sfx;
        public StateMachine subStates;

        public SuperState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
        {
            this.controller = controller;
            this.animator = animator;
            this.particles = particles;
            this.sfx = sfx;

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
