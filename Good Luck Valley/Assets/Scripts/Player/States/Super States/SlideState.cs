using GoodLuckValley.Audio;
using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class SlideState : SuperState
    {
        public SlideState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx) 
            : base(controller, animator, particles, sfx)
        {
        }

        public override void SetupSubStateMachine() { }

        public override void OnEnter()
        {
            // Enter the slide animation and correct the player visual rotation
            animator.EnterSlide();
            animator.CorrectPlayerSlope();

            // Start playing the wall slide sound
            sfx.StartWallSlide();

            // Set the gravity scale
            controller.SetGravityScale(controller.Stats.SlideGravityScale);

            // Set a max speed
            controller.CurrentMaxSpeed = controller.Stats.SlidingMaxSpeed;
            controller.CapSpeed = true;
        }

        public override void OnExit()
        {
            // Remove the max speed cap
            controller.CapSpeed = false;

            // Stop playing the wall slide sound
            sfx.StopWallSlide();

            // Set not sliding
            controller.Collisions.IsSliding = false;
            controller.Active = true;
        }
    }
}
