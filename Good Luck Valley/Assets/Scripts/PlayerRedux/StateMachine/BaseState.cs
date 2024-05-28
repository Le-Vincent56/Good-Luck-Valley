using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;

        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");

        protected const float crossFadeDuration = 0.1f;

        protected BaseState(PlayerController player)
        {
            this.player = player;
            animator = player.GetComponent<Animator>();
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