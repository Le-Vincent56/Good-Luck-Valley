using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

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
        private CrawlState crawling;
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
            crawling = new CrawlState(controller, animator);
            sliding = new SlideState(controller, animator);

            // Define state transitions
            subStates.At(idle, locomotion, new FuncPredicate(() => controller.RB.velocity.x != 0));
            subStates.At(idle, crawling, new FuncPredicate(() => controller.Crawl.Crawling));
            //subStates.At(idle, sliding, new FuncPredicate(() => sliding));

            subStates.At(locomotion, idle, new FuncPredicate(() => controller.RB.velocity.x == 0));
            subStates.At(locomotion, crawling, new FuncPredicate(() => controller.Crawl.Crawling));
            //subStates.At(locomotion, sliding, new FuncPredicate(() => sliding));

            subStates.At(crawling, idle, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x == 0));
            subStates.At(crawling, locomotion, new FuncPredicate(() => !controller.Crawl.Crawling && controller.RB.velocity.x != 0));

            // Set the initial state
            subStates.SetState(idle);
        }
    }

    public class JumpState : SuperState
    {
        public JumpState(PlayerController controller, AnimationController animator)
            : base(controller, animator)
        { }

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
