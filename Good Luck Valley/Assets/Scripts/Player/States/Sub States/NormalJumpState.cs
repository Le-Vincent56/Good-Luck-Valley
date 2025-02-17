using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class NormalJumpState : SubState
    {
        public NormalJumpState(PlayerController controller, AnimationController animator, ParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            // Play the animation
            animator.EnterJump();

            // Play the jump sound
            sfx.Jump();

            // Play the jump particles
            controller.Particles.PlayJumpParticles();
        }
    }
}
