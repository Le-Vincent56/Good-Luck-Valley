using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class CrawlIdleState : SubState
    {
        public CrawlIdleState(PlayerController controller, AnimationController animator, ParticleController particles)
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
    }
}
