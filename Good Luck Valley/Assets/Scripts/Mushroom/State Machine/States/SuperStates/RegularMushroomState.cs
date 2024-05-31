using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachineOld.States
{
    public class RegularMushroomState : MushroomStateOld
    {
        #region FIELDS
        protected bool isBouncing;
        #endregion

        public RegularMushroomState(MushroomControllerOld mushroom, MushroomStateMachineOld stateMachine, string animationBoolName)
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

            isBouncing = mushroom.CheckIfBouncing();
        }
    }
}
