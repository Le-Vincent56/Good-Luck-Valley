using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerBounceState : PlayerAbilityState
    {
        private bool isBouncing;

        public PlayerBounceState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
                : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }

        public override void Enter()
        {
            base.Enter();

            if (!isBouncing)
            {
                isBouncing = true;
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Jump Checks
            if (isBouncing && player.RB.velocity.y < 0.01f)
            {
                // Set bouncing to false
                isBouncing = false;

                // End bounce
                player.EndBounce();

                // Trigger ability done
                isAbilityDone = true;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Move the player in the air
            player.Move(0.5f, true);
        }
    }
}
