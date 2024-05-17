using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine.States
{
    public class WMIdleState : WallMushroomState
    {
        public WMIdleState(MushroomController mushroom, MushroomStateMachine stateMachine, string animationBoolName) 
            : base(mushroom, stateMachine, animationBoolName)
        {
        }
    }
}