using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class NoClipState : SuperState
    {
        public NoClipState(PlayerController controller, AnimationController animator, ParticleController particles, PlayerSFX sfx)
            : base(controller, animator, particles, sfx)
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
