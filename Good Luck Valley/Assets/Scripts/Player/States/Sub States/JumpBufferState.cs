using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Player.States
{
    public class JumpBufferState : SubState
    {
        public JumpBufferState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
            : base(controller, animator, particles)
        { }
    }
}
