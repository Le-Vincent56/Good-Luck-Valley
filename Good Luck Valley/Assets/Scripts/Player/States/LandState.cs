using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class LandState : BaseState
    {
        private readonly PlayerSFXHandler sfx;

        public LandState(PlayerController player, Animator animator, PlayerSFXHandler sfx) : base(player, animator)
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Play the land sound effect
            sfx.Land();
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