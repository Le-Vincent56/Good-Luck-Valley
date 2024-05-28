using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

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