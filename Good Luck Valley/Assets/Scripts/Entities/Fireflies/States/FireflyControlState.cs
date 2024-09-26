using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class FireflyControlState : IState
    {
        protected readonly FireflyController fireflies;
        protected readonly Animator animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int FollowHash = Animator.StringToHash("Bounce");

        protected const float crossFadeDuration = 0.1f;

        public FireflyControlState(FireflyController fireflies, Animator animator)
        {
            this.fireflies = fireflies;
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