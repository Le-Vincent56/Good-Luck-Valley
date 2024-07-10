using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class WallJumpState : BaseState
    {
        private readonly PlayerSFXMaster sfx;

        public WallJumpState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            animator.CrossFade(WallJumpHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // Play the wall jump sound effect
            sfx.WallJump();
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

        public override void OnExit()
        {
            player.SetWallJumping(false);
        }
    }
}