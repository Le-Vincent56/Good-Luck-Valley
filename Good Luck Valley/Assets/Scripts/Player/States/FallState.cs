using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class FallState : BaseState
    {
        public FallState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);

            // Reset the player bounce
            player.ResetBounce();
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