using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class FallState : SuperState
    {
        private NormalFallState normalFall;
        private FastFallState fastFall;
        private SlowFallState slowFall;

        public FallState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
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
            normalFall = new NormalFallState(controller, animator, particles);
            fastFall = new FastFallState(controller, animator, particles);
            slowFall = new SlowFallState(controller, animator, particles);

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
