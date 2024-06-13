using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class FallState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public FallState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);

            // Reset the player bounce
            player.ResetBounce();

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // Play the falling sound effect
            sfx.Fall();
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