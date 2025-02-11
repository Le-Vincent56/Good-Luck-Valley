using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class SlowFallState : SubState
    {
        public SlowFallState(PlayerController controller, AnimationController animator, ParticleController particles)
        : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            // Set gravity
            controller.SetGravityScale(controller.Stats.FallGravityScale * controller.Stats.SlowFallMultiplier);

            // Play the float particles
            particles.PlayFloatParticles();
        }

        public override void OnExit()
        {
            // Stop the float particles
            particles.StopFloatParticles();
        }
    }
}
