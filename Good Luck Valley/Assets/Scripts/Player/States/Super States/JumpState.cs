using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class JumpState : SuperState
    {
        private NormalJumpState normalJump;
        private WarpJumpState warpJump;
        private WallJumpState wallJump;
        private JumpBufferState buffer;

        public JumpState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            if (controller.WallJump.IsWallJumping)
            {
                subStates.SetState(wallJump);
            } else if (controller.Jump.WarpJumping)
            {
                subStates.SetState(warpJump);
            }
            else
            {
                subStates.SetState(normalJump);
            }

            // Set controller variables
            controller.RB.gravityScale = controller.Stats.JumpGravityScale;

            // Correct player rotation
            animator.CorrectPlayerRotation();
        }

        public override void SetupSubStateMachine()
        {
            // Initialize state machine
            subStates = new StateMachine();

            // Create states
            normalJump = new NormalJumpState(controller, animator, particles, sfx);
            warpJump = new WarpJumpState(controller, animator, particles, sfx);
            wallJump = new WallJumpState(controller, animator, particles, sfx);
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
        }
    }
}
