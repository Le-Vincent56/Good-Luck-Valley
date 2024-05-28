using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GoodLuckValley.Player.StateMachine.States
{
    public enum SlopeDirection
    {
        None = 0,
        AscentFromGround = 1,
        AscentToGround = 2,
        DescentFromGround = 3,
        DescentToGround = 4,
        AscentOnSlope = 5,
        DescentOnSlope = 6,
    }

    public class PlayerSlopeState : PlayerGroundedState
    {
        public SlopeDirection Direction { get; set; }
        public float Angle { get; set; }
        public Vector2 ContactPoint { get; set; }

        public PlayerSlopeState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            if (Direction == SlopeDirection.DescentOnSlope) player.RB.velocity = Vector2.zero;
        }

        public override void Exit()
        {
            base.Exit();

            player.RB.velocity = Vector2.zero;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Exit case - idle
            if(Direction == SlopeDirection.AscentToGround)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            if(xInput == 0f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            if(!isOnSlope && xInput != 0f)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            MoveOnSlope(0.5f);
        }

        public void MoveOnSlope(float lerpAmount)
        {
            if (player.CheckIfNoClip())
            {
                float xMovement = player.InputHandler.NormInputX * player.NoClipSpeed;
                float yMovement = player.InputHandler.NormInputY * player.NoClipSpeed;

                player.transform.position += Vector3.right * xMovement;
                player.transform.position += Vector3.up * yMovement;

                player.CheckDirectionToFace(player.InputHandler.NormInputX > 0);

                return;

            }

            // Calculate the direction we want to move in and our desired velocity
            float targetSpeed = player.InputHandler.NormInputX * playerData.runMaxSpeed;

            // Reduce our control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(player.RB.velocity.x, targetSpeed, lerpAmount);

            // Calculate accelRate
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;

            // Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - player.RB.velocity.x;

            // Calculate force along x-axis to apply to the player
            float movement = speedDif * accelRate;

            // Calculate movement vector
            Vector2 slopeVector = new Vector2(
                Mathf.Sin(Angle * Mathf.Deg2Rad),
                Mathf.Cos(Angle * Mathf.Deg2Rad)
            ).normalized;

            switch(Direction)
            {
                case SlopeDirection.AscentFromGround:
                    slopeVector.y *= Mathf.Sign(xInput);
                    break;

                case SlopeDirection.AscentOnSlope:
                    slopeVector.y *= Mathf.Sign(xInput);
                    break;

                case SlopeDirection.DescentFromGround:
                    slopeVector.y *= -Mathf.Sign(xInput);
                    break;

                case SlopeDirection.DescentOnSlope:
                    slopeVector.y *= -Mathf.Sign(xInput);
                    break;
            }

            Debug.DrawRay(new Vector2(ContactPoint.x, ContactPoint.y + 0.25f), slopeVector * movement, Color.yellow);

            // Convert this to a vector and apply to rigidbody
            player.RB.AddForce(movement * slopeVector, ForceMode2D.Force);

            // Turn the player
            player.CheckDirectionToFace(player.InputHandler.NormInputX > 0);
        }
    }
}