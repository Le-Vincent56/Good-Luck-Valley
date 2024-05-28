using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player) : base(player) { }

        public override void OnEnter()
        {
            animator.CrossFade(JumpHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            // Call Player's jump logic and move logic
        }
    }
}