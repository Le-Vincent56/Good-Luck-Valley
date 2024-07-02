using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LandState : BaseState
    {
        private readonly PlayerSFXHandler sfx;
        private readonly PlayerParticlesController particles;

        public LandState(PlayerController player, Animator animator, PlayerSFXHandler sfx, PlayerParticlesController particles) : base(player, animator)
        {
            this.sfx = sfx;
            this.particles = particles;
        }

        public override void OnEnter()
        {
            // Play the land sound effect
            sfx.Land();

            // Play land particles
            particles.HandleLandParticles();
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