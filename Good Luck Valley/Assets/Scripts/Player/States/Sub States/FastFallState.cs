using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class FastFallState : SubState
    {
        public FastFallState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Set the gravity scale
            controller.SetGravityScale(controller.Stats.FallGravityScale * controller.Stats.FastFallMultiplier);
        }
    }
}
