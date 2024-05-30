using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachineOld.States
{
    public class WMIdleState : WallMushroomState
    {
        public WMIdleState(MushroomControllerOld mushroom, MushroomStateMachineOld stateMachine, string animationBoolName) 
            : base(mushroom, stateMachine, animationBoolName)
        {
        }
    }
}