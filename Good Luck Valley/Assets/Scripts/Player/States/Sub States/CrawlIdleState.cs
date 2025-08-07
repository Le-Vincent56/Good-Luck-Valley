using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class CrawlIdleState : SubState
    {
        public CrawlIdleState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Enter the crawl idle animation
            animator.EnterCrawlIdle();
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
        }
    }
}
