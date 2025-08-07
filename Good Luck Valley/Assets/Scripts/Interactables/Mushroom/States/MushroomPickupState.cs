using GoodLuckValley.Architecture.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Interactables.Mushroom.States
{
    public class MushroomPickupState : IState
    {
        protected readonly MushroomPickup pickup;
        protected readonly Animator animator;
        protected readonly ParticleSystem particleSystem;
        
        protected static readonly int IDLE_HASH = Animator.StringToHash("Idle");
        protected static readonly int EFFECT_HASH = Animator.StringToHash("Effect");
        protected const float FADE_DURATION = 0.2f;
        
        protected MushroomPickupState(MushroomPickup pickup, Animator animator, ParticleSystem particleSystem)
        {
            this.pickup = pickup;
            this.animator = animator;
            this.particleSystem = particleSystem;
        }
        
        public virtual void OnEnter() { /* Noop */ }
        public virtual void Update() { /* Noop */ }
        public virtual void FixedUpdate() { /* Noop */ }
        public virtual void OnExit() { /* Noop */ }
    }
}
