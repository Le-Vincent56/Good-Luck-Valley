using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class IdleState : BaseState
    {
        public IdleState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);

            // Allow the player to peek
            player.SetCanPeek(true);
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Handle wall sliding
            player.HandleWallSliding();

            // Handle player movement
            player.HandleMovement();
        }
    }
}