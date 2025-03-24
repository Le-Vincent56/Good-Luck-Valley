using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class IdleState : SubState
    {
        public IdleState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Enter the idle animation
            animator.EnterIdle();
        }
    }
}
