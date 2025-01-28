using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class NormalJumpState : SubState
    {
        public NormalJumpState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Play the animation
            animator.EnterJump();

            // Play the jump particles
            particles.PlayJumpParticles();
        }
    }
}
