using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class BounceState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public BounceState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // Play the bounce sound effect
            sfx.Bounce();
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Calculate wall sliding
            player.HandleWallSliding();

            // Handle player movement
            player.HandleMovement();
        }
    }
}