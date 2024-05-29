using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(JumpHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Calculate wall sliding
            player.HandleWallSliding();

            // Handle movement
            player.HandleMovement();
        }
    }
}