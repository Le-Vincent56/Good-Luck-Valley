using GoodLuckValley.VFX.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using System.Diagnostics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace GoodLuckValley.Player.States
{
    public class NormalFallState : SubState
    {
        public NormalFallState(PlayerController controller, AnimationController animator, PlayerParticleController particles)
            : base(controller, animator, particles)
        { }

        public override void OnEnter()
        {
            controller.SetGravityScale(controller.Stats.FallGravityScale);
        }
    }
}
