using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class FallState : BaseState
    {
        public FallState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(FallHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Calculate velocity
            player.CalculateVelocity();

            // Handle player movement
            player.HandleMovement();
        }
    }
}