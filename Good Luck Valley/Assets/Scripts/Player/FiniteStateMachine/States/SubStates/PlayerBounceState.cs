using GoodLuckValley.Player.StateMachine;
using GoodLuckValley.Player.StateMachine.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounceState : PlayerAbilityState
{
    private bool isBouncing;


    public PlayerBounceState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName)
            : base(player, stateMachine, playerData, animationaBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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

        // Move the player in the air
        player.Move(0.5f, true);
    }
}
