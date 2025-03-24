using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class CrawlLocomotionState : SubState
    {
        public CrawlLocomotionState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        {
        }

        public override void OnEnter()
        {
            // Enter the crawl locomotion animation
            animator.EnterCrawlLocomotion();

            // Start playing footsteps with crawl RTPC values
            sfx.SetSpeedRTPC(sfx.CRAWL);
            sfx.StartGroundImpacts();
        }

        public override void Update()
        {
            // Rotate the player to align with the ground
            animator.RotatePlayer();
        }

        public override void OnExit()
        {
            // Correct player rotation
            animator.CorrectPlayerRotation();

            // Stop playing footsteps
            sfx.StopGroundImpacts();
        }
    }
}
