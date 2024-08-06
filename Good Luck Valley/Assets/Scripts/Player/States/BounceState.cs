using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class BounceState : BaseState
    {
        public BounceState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Calculate wall sliding
            player.HandleWallSliding();

            // Handle player movement
            player.HandleMovement();

            Debug.Log(player.Velocity);
        }
    }
}