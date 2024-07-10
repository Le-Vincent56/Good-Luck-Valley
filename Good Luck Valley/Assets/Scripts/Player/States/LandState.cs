using GoodLuckValley.Audio.SFX;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LandState : BaseState
    {
        private readonly PlayerSFXMaster sfx;

        public LandState(PlayerController player, Animator animator, PlayerSFXMaster sfx) : base(player, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Play the land sound effect
            sfx.Land();

            // Play land particles
            player.PlayParticles(1);
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