using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerInAirState : PlayerState
    {
        public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
            : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

    }
}