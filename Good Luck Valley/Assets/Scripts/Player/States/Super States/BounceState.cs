using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class BounceState : SuperState
    {
        public BounceState(PlayerController controller, AnimationController animator, PlayerParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
        { }

        public override void OnEnter()
        {
            // Enter the Bounce animation
            animator.EnterBounce();

            // Correct the player rotation
            animator.CorrectPlayerRotation();

            // Set controller variables
            controller.RB.gravityScale = controller.Stats.BounceGravityScale;
        }

        public override void SetupSubStateMachine() { }
    }
}
