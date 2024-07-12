using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class JumpState : BaseState
    {
        private readonly PlayerSFXMaster sfx;

        public JumpState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            player.LearnControl("Jump");

            animator.CrossFade(JumpHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // Play the jump sound effect
            sfx.Jump();

            // Play jump particles
            player.PlayParticles(0);
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