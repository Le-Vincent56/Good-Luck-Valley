using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachineOld.States
{
    public class WallMushroomState : MushroomStateOld
    {
        public WallMushroomState(MushroomControllerOld mushroom, MushroomStateMachineOld stateMachine, string animationBoolName)
            : base(mushroom, stateMachine, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }
    }
}
