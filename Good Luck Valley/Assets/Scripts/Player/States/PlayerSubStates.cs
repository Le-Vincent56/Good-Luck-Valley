using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class SubState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        public StateMachine subStates;

        public SubState(PlayerController controller, AnimationController animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnExit() { }
    }

    public class IdleState : SubState
    {
        public IdleState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the idle animation
            animator.EnterIdle();
        }
    }

    public class LocomotionState : SubState
    {
        public LocomotionState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the locomotion animation
            animator.EnterLocomotion();
        }
    }

    public class CrawlIdleState : SubState
    {
        public CrawlIdleState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the crawl idle animation
            animator.EnterCrawlIdle();
        }
    }

    public class CrawlLocomotionState : SubState
    {
        public CrawlLocomotionState(PlayerController controller, AnimationController animator) 
            : base(controller, animator)
        {
        }

        public override void OnEnter()
        {
            // Enter the crawl locomotion animation
            animator.EnterCrawlLocomotion();
        }
    }

    public class NormalJumpState : SubState
    {
        public NormalJumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            animator.EnterJump();
        }
    }

    public class WarpJumpState : SubState
    {
        public WarpJumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            animator.EnterWarpJump();
        }
    }

    public class WallJumpState : SubState
    {
        public WallJumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator) 
        { }

        public override void OnEnter()
        {
            animator.EnterWallJump();
        }
    }

    public class JumpBufferState : SubState
    {
        public JumpBufferState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }
    }

    public class NormalFallState : SubState
    {
        public NormalFallState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            controller.SetGravityScale(controller.InitialGravityScale);
        }
    }

    public class FastFallState : SubState
    {
        private float initialConstantGravity;

        public FastFallState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Set the gravity scale
            controller.SetGravityScale(controller.InitialGravityScale * controller.Stats.FastFallMultiplier);
            initialConstantGravity = controller.ExtraConstantGravity;
            controller.ExtraConstantGravity = initialConstantGravity * controller.Stats.FastFallMultiplier;
        }

        public override void OnExit()
        {
            // Reset the initial constant gravity
            controller.ExtraConstantGravity = initialConstantGravity;
        }
    }

    public class SlowFallState : SubState
    {
        private float initialConstantGravity;

        public SlowFallState(PlayerController controller, AnimationController animator) 
            : base(controller, animator) 
        { }

        public override void OnEnter()
        {
            // Set gravity
            controller.SetGravityScale(controller.InitialGravityScale * controller.Stats.SlowFallMultiplier);
            initialConstantGravity = controller.ExtraConstantGravity;
            controller.ExtraConstantGravity = initialConstantGravity * controller.Stats.SlowFallMultiplier;
        }

        public override void OnExit()
        {
            // Reset the initial constant gravity
            controller.ExtraConstantGravity = initialConstantGravity;
        }
    }
}
