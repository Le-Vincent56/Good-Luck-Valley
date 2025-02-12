using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class WallState : SuperState
    {
        public WallState(PlayerController controller, AnimationController animator, ParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            animator.EnterWallSlide();

            // Start the wall slide particles
            particles.PlayWallSlideParticles();

            // Start playing the wall slide sound
            sfx.StartWallSlide();

            controller.RB.gravityScale = controller.Stats.WallSlideGravityScale;
        }

        public override void SetupSubStateMachine() { }

        public override void OnExit()
        {
            // Stop the wall slide particles
            particles.StopWallSlideParticles();

            // Stop playing the wall slide sound
            sfx.StopWallSlide();

            // Set the gravity scale back to normal
            controller.RB.gravityScale = controller.Stats.JumpGravityScale;
        }
    }
}
