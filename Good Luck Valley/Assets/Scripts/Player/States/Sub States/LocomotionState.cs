using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class LocomotionState : SubState
    {
        public LocomotionState(PlayerController controller, AnimationController animator, ParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            // Enter the locomotion animation
            animator.EnterLocomotion();

            // Play running particles
            particles.PlayRunningParticles();

            // Start playing ground impacts sounds with the run value for the speed RTPC
            sfx.SetSpeedRTPC(sfx.RUN);
            sfx.StartGroundImpacts();
        }

        public override void OnExit()
        {
            // Stop playing running particles
            particles.StopRunningParticles();

            // Stop playing footsteps
            sfx.StopGroundImpacts();
        }
    }
}
