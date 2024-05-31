using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.States
{
    public class WallJumpState : BaseState
    {
        public WallJumpState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(WallJumpHash, crossFadeDuration);
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