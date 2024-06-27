using GoodLuckValley.Player.Control;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int WallSlideHash = Animator.StringToHash("Wall Slide");
        protected static readonly int FallHash = Animator.StringToHash("Fall");
        protected static readonly int BounceHash = Animator.StringToHash("Bounce");
        protected static readonly int WallJumpHash = Animator.StringToHash("Wall Jump");
        protected static readonly int ThrowIdleHash = Animator.StringToHash("Throw Idle");
        protected static readonly int ThrowLocomotionHash = Animator.StringToHash("Throw Locomotion");
        protected static readonly int CrawlIdleHash = Animator.StringToHash("Crawl Idle");
        protected static readonly int CrawlLocomotionHash = Animator.StringToHash("Crawl Locomotion");

        protected const float crossFadeDuration = 0.1f;

        public BaseState(PlayerController player, Animator animator)
        {
            this.player = player;
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