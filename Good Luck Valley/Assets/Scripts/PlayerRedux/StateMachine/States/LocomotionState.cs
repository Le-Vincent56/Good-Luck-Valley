using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController player) : base(player) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            // Call Player's jump logic and move logic
            base.FixedUpdate();
        }
    }
}