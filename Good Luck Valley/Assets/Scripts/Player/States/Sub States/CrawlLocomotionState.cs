using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class CrawlLocomotionState : SubState
    {
        public CrawlLocomotionState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        {
        }

        public override void OnEnter()
        {
            // Enter the crawl locomotion animation
            animator.EnterCrawlLocomotion();
        }
    }
}
