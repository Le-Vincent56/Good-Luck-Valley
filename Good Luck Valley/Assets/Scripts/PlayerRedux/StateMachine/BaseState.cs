using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
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

        protected const float crossFadeDuration = 0.1f;

        protected BaseState(PlayerController player, Animator animator)
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