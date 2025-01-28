using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class NoClipState : SuperState
    {
        public NoClipState(PlayerController controller, AnimationController animator, ParticleController particles)
            : base(controller, animator, particles)
        { }
        public override void SetupSubStateMachine() { }

        public override void OnEnter()
        {
            controller.RB.gravityScale = 0f;
            controller.SetVelocity(Vector2.zero);
            controller.ConstantForce.force = Vector2.zero;
        }
    }
}
