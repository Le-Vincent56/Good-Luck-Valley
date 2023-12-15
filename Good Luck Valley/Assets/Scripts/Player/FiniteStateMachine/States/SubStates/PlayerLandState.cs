using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

    }
}
