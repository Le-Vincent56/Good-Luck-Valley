using GoodLuckValley.Patterns.StateMachine;
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
            subStates?.Update();
        }

        public virtual void FixedUpdate()
        {
            // Update the sub-StateMachine
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
        private SlideState sliding;

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
            sliding = new SlideState(controller, animator);

            // Define state transitions
            subStates.At(idle, locomotion, new FuncPredicate(() => controller.RB.velocity.x != 0));
            subStates.At(idle, crawlingIdle, new FuncPredicate(() => controller.Crawl.Crawling));
            subStates.At(idle, crawlingLocomotion, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.velocity.x != 0));
            subStates.At(idle, sliding, new FuncPredicate(() => controller.Slide.Sliding));

            subStates.At(locomotion, idle, new FuncPredicate(() => controller.RB.velocity.x == 0));
            subStates.At(locomotion, crawlingIdle, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.velocity.x == 0));
            subStates.At(locomotion, crawlingLocomotion, new FuncPredicate(() => controller.Crawl.Crawling && controller.RB.velocity.x != 0));
            subStates.At(locomotion, sliding, new FuncPredicate(() => controller.Slide.Sliding));

            subStates.At(crawlingIdle, idle, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x == 0));
            subStates.At(crawlingIdle, locomotion, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x != 0));
            subStates.At(crawlingIdle, crawlingLocomotion, new FuncPredicate(() => controller.RB.velocity.x != 0));
            subStates.At(crawlingIdle, sliding, new FuncPredicate(() => controller.Slide.Sliding));

            subStates.At(crawlingLocomotion, idle, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x == 0));
            subStates.At(crawlingLocomotion, locomotion, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x != 0));
            subStates.At(crawlingLocomotion, crawlingIdle, new FuncPredicate(() => controller.RB.velocity.x == 0));
            subStates.At(crawlingLocomotion, sliding, new FuncPredicate(() => controller.Slide.Sliding));

            subStates.At(sliding, idle, new FuncPredicate(() => !controller.Slide.Sliding && controller.RB.velocity.x == 0));
            subStates.At(sliding, locomotion, new FuncPredicate(() => !controller.Slide.Sliding && controller.RB.velocity.x != 0));
            subStates.At(sliding, crawlingIdle, new FuncPredicate(() => !controller.Slide.Sliding && controller.Crawl.Crawling && controller.RB.velocity.x == 0));
            subStates.At(sliding, crawlingLocomotion, new FuncPredicate(() => !controller.Slide.Sliding && controller.Crawl.Crawling && controller.RB.velocity.x == 0));

            // Set the initial state
            subStates.SetState(idle);
        }
    }

    public class JumpState : SuperState
    {
        public JumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

        public override void OnEnter()
        {
            // Enter the jump animation
            animator.EnterJump();
        }

        public override void SetupSubStateMachine() { }
    }

    public class FallState : SuperState
    {
        private NormalFallState normalFall;
        private FastFallState fastFall;

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

            // Define state transitions
            subStates.At(normalFall, fastFall, new FuncPredicate(() => controller.FrameData.Input.Move.y < 0));
            subStates.At(fastFall, normalFall, new FuncPredicate(() => controller.FrameData.Input.Move.y == 0));

            // Set an initial state
            subStates.SetState(normalFall);
        }
    }
}
