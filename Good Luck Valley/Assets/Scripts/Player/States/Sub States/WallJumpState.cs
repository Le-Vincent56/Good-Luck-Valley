using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class WallJumpState : SubState
    {
        public WallJumpState(PlayerController controller, AnimationController animator, ParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            // Enter the wall jump animation
            animator.EnterWallJump();

            // Play the jump sound
            sfx.Jump();

            // Play the wall jump particles
            particles.PlayWallJumpParticles();
        }
    }
}
