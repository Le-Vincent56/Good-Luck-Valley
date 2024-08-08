using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Mushroom.States
{
    public class MushroomState : IState
    {
        protected readonly MushroomController mushroom;
        protected readonly Animator animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BounceHash = Animator.StringToHash("Bounce");
        protected static readonly int GrowHash = Animator.StringToHash("Grow");
        protected static readonly int DissipateHash = Animator.StringToHash("Dissipate");

        protected const float crossFadeDuration = 0.1f;

        protected MushroomState(MushroomController mushroom, Animator animator)
        {
            this.mushroom = mushroom;
            this.animator = animator;
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