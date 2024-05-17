using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine.States
{
    public class RMIdleState : RegularMushroomState
    {
        public RMIdleState(MushroomController mushroom, MushroomStateMachine stateMachine, string animationBoolName) 
            : base(mushroom, stateMachine, animationBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Exit case - applied bounce
            if(isBouncing)
            {
                stateMachine.ChangeState(mushroom.RMBounceState);
            }
        }
    }
}
