using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class GroundedState : SuperState
    {
        private IdleState idle;
        private LocomotionState locomotion;
        private CrawlIdleState crawlingIdle;
        private CrawlLocomotionState crawlingLocomotion;

        public GroundedState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
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
            idle = new IdleState(controller, animator, particles);
            locomotion = new LocomotionState(controller, animator, particles);
            crawlingIdle = new CrawlIdleState(controller, animator, particles);
            crawlingLocomotion = new CrawlLocomotionState(controller, animator, particles);

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

        public override void OnExit()
        {
            // Stop playing the running particles
            particles.StopRunningParticles();
        }
    }
}
