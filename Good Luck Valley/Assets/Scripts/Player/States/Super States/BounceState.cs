using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class BounceState : SuperState
    {
        public BounceState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Enter the Bounce animation
            animator.EnterBounce();

            // Set controller variables
            controller.RB.gravityScale = controller.Stats.BounceGravityScale;
        }

        public override void SetupSubStateMachine() { }
    }
}
