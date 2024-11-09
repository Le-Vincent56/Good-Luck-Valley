using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley
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

    public class SlideState : SubState
    {
        public SlideState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            animator.EnterSlide();
        }
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
        public FastFallState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            controller.SetGravityScale(controller.InitialGravityScale * controller.Stats.FastFallMultiplier);
        }
    }
}
