using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public abstract class SuperState : IState
    {
        protected readonly PlayerController controller;
        protected readonly AnimationController animator;
        public StateMachine subStates;

        public SuperState(PlayerController controller, AnimationController animator)
        {
            this.controller = controller;
            this.animator = animator;

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

    public class GroundedState : SuperState
    {
        private IdleState idle;
        private LocomotionState locomotion;
        private CrawlIdleState crawlingIdle;
        private CrawlLocomotionState crawlingLocomotion;

        public GroundedState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Set the idle as the default state
            subStates.SetState(idle);
        }

        public override void SetupSubStateMachine() 
        {
            // Initialize the State Machine
            subStates = new StateMachine();

            // Create states
            idle = new IdleState(controller, animator);
            locomotion = new LocomotionState(controller, animator);
            crawlingIdle = new CrawlIdleState(controller, animator);
            crawlingLocomotion = new CrawlLocomotionState(controller, animator);

            // Define state transitions
            subStates.At(idle, locomotion, new FuncPredicate(() => controller.RB.linearVelocity.x != 0));
            subStates.At(idle, crawlingIdle, new FuncPredicate(() => controller.Crawl.Crawling));
            subStates.At(idle, crawlingLocomotion, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.linearVelocity.x != 0));

            subStates.At(locomotion, idle, new FuncPredicate(() => controller.RB.linearVelocity.x == 0));
            subStates.At(locomotion, crawlingIdle, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.linearVelocity.x == 0));
            subStates.At(locomotion, crawlingLocomotion, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.linearVelocity.x != 0));

            subStates.At(crawlingIdle, idle, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.linearVelocity.x == 0));
            subStates.At(crawlingIdle, locomotion, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.linearVelocity.x != 0));
            subStates.At(crawlingIdle, crawlingLocomotion, new FuncPredicate(() => controller.RB.linearVelocity.x != 0));

            subStates.At(crawlingLocomotion, idle, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.linearVelocity.x == 0));
            subStates.At(crawlingLocomotion, locomotion, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.linearVelocity.x != 0));
            subStates.At(crawlingLocomotion, crawlingIdle, new FuncPredicate(() => controller.RB.linearVelocity.x == 0));

            // Set the initial state
            subStates.SetState(idle);
        }
    }

    public class WallState : SuperState
    {
        public WallState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            animator.EnterWallSlide();
        }

        public override void SetupSubStateMachine() { }
    }

    public class JumpState : SuperState
    {
        private NormalJumpState normalJump;
        private WallJumpState wallJump;

        public JumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Set the normal jump on default
            subStates.SetState(normalJump);

            // Set controller variables
            controller.RB.gravityScale = controller.InitialGravityScale;
            controller.ExtraConstantGravity = controller.Stats.JumpConstantGravity;
        }

        public override void SetupSubStateMachine() 
        {
            // Initialize state machine
            subStates = new StateMachine();

            // Create states
            normalJump = new NormalJumpState(controller, animator);
            wallJump = new WallJumpState(controller, animator);

            // Define state transitions
            subStates.At(normalJump, wallJump, new FuncPredicate(() => controller.WallJump.IsWallJumping));

            // Set the initial state
            subStates.SetState(normalJump);
        }
    }

    public class BounceState : SuperState
    {
        public BounceState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the Bounce animation
            animator.EnterBounce();

            // Set controller variables
            controller.RB.gravityScale = controller.InitialGravityScale;
            controller.ExtraConstantGravity = controller.Stats.BounceConstantGravity;
        }

        public override void OnExit()
        {
            // Reset bounce variables
            controller.Bounce.ResetBounce(true);
        }

        public override void SetupSubStateMachine() { }
    }

    public class FallState : SuperState
    {
        private NormalFallState normalFall;
        private FastFallState fastFall;
        private SlowFallState slowFall;

        public FallState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the fall animation
            animator.EnterFall();

            // Set the normal fall as the default state
            subStates.SetState(normalFall);
        }

        public override void SetupSubStateMachine()
        {
            // Initialize the State Machine
            subStates = new StateMachine();

            // Create states
            normalFall = new NormalFallState(controller, animator);
            fastFall = new FastFallState(controller, animator);
            slowFall = new SlowFallState(controller, animator);

            // Define state transitions
            subStates.At(normalFall, fastFall, new FuncPredicate(() => controller.FrameData.Input.Move.y < 0));
            subStates.At(fastFall, normalFall, new FuncPredicate(() => controller.FrameData.Input.Move.y == 0));
            subStates.At(normalFall, slowFall, new FuncPredicate(() => controller.Bounce.CanSlowFall && controller.FrameData.Input.SlowFalling));
            subStates.At(slowFall, normalFall, new FuncPredicate(() => !controller.Bounce.CanSlowFall || !controller.FrameData.Input.SlowFalling));

            // Set an initial state
            subStates.SetState(normalFall);
        }
    }
}
