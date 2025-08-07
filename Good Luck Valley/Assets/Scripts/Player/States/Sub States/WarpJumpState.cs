using GoodLuckValley.Audio;
using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class WarpJumpState : SubState
    {
        public WarpJumpState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            // Play the double jump sound
            sfx.DoubleJump();

            animator.EnterWarpJump();
        }
    }
}
