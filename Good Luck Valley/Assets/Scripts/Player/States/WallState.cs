
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class WallState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public WallState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(WallSlideHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // TODO: Play wall slide sound effect
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