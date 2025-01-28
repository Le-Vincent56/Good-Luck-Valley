using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class WarpJumpState : SubState
    {
        public WarpJumpState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            animator.EnterWarpJump();
        }
    }
}
