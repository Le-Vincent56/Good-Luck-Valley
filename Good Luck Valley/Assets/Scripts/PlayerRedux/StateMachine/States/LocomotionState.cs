using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            //player.Move();
        }
    }
}