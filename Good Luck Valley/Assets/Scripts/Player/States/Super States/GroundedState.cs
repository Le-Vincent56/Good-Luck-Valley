using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class GroundedState : SuperState
    {
        private IdleState idle;
        private LocomotionState locomotion;
        private CrawlIdleState crawlingIdle;
        private CrawlLocomotionState crawlingLocomotion;
        private StateMachine stateMachine;

        public GroundedState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx, StateMachine stateMachine)
            : base(controller, animator, particles, sfx)
        {
            this.stateMachine = stateMachine;
        }

        public override void OnEnter()
        {
            // Correct the rotation
            animator.CorrectPlayerRotation();

            // Set the idle as the default state
            subStates.SetState(idle);

            // Check if the previous state was the Wall State
            if (stateMachine.GetPreviousState() is WallState)
                // Play the Wall Slide End impact
                sfx.PlayWallSlideEnd();
        }

        public override void SetupSubStateMachine()
        {
            // Initialize the State Machine
            subStates = new StateMachine();

            // Create states
            idle = new IdleState(controller, animator, particles);
            locomotion = new LocomotionState(controller, animator, particles, sfx);
            crawlingIdle = new CrawlIdleState(controller, animator, particles);
            crawlingLocomotion = new CrawlLocomotionState(controller, animator, particles, sfx);

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

            // Stop playing ground impacts
            sfx.StopGroundImpacts();
        }
    }
}
