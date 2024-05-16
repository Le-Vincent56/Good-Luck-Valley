using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine
{
    public class RegularMushroomState : MushroomState
    {
        #region FIELDS
        protected bool isBouncing;
        #endregion

        public RegularMushroomState(MushroomController mushroom, MushroomStateMachine stateMachine, string animationBoolName)
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
