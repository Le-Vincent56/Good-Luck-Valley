using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class JumpState : SuperState
    {
        private NormalJumpState normalJump;
        private WarpJumpState warpJump;
        private WallJumpState wallJump;
        private JumpBufferState buffer;

        public JumpState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Set the normal jump on default
            subStates.SetState(normalJump);

            // Set controller variables
            controller.RB.gravityScale = controller.Stats.JumpGravityScale;
        }

        public override void SetupSubStateMachine()
        {
            // Initialize state machine
            subStates = new StateMachine();

            // Create states
            normalJump = new NormalJumpState(controller, animator, particles);
            warpJump = new WarpJumpState(controller, animator, particles);
            wallJump = new WallJumpState(controller, animator, particles);
            buffer = new JumpBufferState(controller, animator, particles);

            // Define state transitions
            subStates.At(normalJump, wallJump, new FuncPredicate(() => controller.WallJump.IsWallJumping));
            subStates.At(normalJump, warpJump, new FuncPredicate(() => controller.Jump.WarpJumping));

            subStates.At(warpJump, buffer, new FuncPredicate(() => !controller.Jump.WarpJumping));
            subStates.At(warpJump, wallJump, new FuncPredicate(() => controller.WallJump.IsWallJumping));

            subStates.At(wallJump, warpJump, new FuncPredicate(() => controller.Jump.WarpJumping));
            subStates.At(wallJump, buffer, new FuncPredicate(() => !controller.WallJump.IsWallJumping));

            subStates.At(buffer, warpJump, new FuncPredicate(() => controller.Jump.WarpJumping));
            subStates.At(buffer, wallJump, new FuncPredicate(() => controller.WallJump.IsWallJumping));

            // Set the initial state
            subStates.SetState(normalJump);
        }
    }
}
