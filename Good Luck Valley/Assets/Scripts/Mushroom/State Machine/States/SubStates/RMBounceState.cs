using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine.States
{
    public class RMBounceState : RegularMushroomState
    {
        public RMBounceState(MushroomController mushroom, MushroomStateMachine stateMachine, string animationBoolName)
            : base(mushroom, stateMachine, animationBoolName)
        {
        }

        public override void Exit()
        {
            base.Exit();

            // Set the Mushroom Controller's isBouncing to false
            mushroom.SetIsBouncing(false);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Exit case - bounce animation finished
            if(isAnimationFinished)
            {
                // Set isBouncing to false
                isBouncing = false;

                // Change back to idle
                stateMachine.ChangeState(mushroom.RMIdleState);
            }
        }
    }
}
