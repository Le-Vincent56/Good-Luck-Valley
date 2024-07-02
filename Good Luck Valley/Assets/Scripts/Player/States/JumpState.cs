using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class JumpState : BaseState
    {
        private readonly PlayerSFXHandler sfx;
        private readonly PlayerParticlesController particles;

        public JumpState(PlayerController player, Animator animator, PlayerSFXHandler sfx, PlayerParticlesController particles) : base(player, animator) 
        {
            this.sfx = sfx;
            this.particles = particles;
        }

        public override void OnEnter()
        {
            player.LearnControl("Jump");

            animator.CrossFade(JumpHash, crossFadeDuration);

            // Don't allow the player to peek
            player.SetCanPeek(false);

            // TODO: Play the jumping sound effect

            // Play jump particles
            particles.HandleJumpParticles();
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