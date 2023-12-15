using GoodLuckValley.Player.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerState
    {
        #region FIELDS
        protected Player player;
        protected PlayerStateMachine stateMachine;
        protected PlayerData playerData;

        protected float startTime;

        private string animationBoolName;
        #endregion

        public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.playerData = playerData;
            this.animationBoolName = animationaBoolName;
        }

        /// <summary>
        /// Enter the player state
        /// </summary>
        public virtual void Enter()
        {
            // Check states
            DoChecks();

            // Set the animation state
            player.Anim.SetBool(animationBoolName, true);

            // Set start time
            startTime = Time.time;

            Debug.Log(animationBoolName);
        }

        /// <summary>
        /// Exit the player state
        /// </summary>
        public virtual void Exit()
        {
            // Leave the animation state
            player.Anim.SetBool(animationBoolName, false);
        }
        
        /// <summary>
        /// Update the player logic
        /// </summary>
        public virtual void LogicUpdate()
        {

        }

        /// <summary>
        /// Update the player physics
        /// </summary>
        public virtual void PhysicsUpdate()
        {
            // Check states
            DoChecks();
        }

        /// <summary>
        /// Run certain checks
        /// </summary>
        public virtual void DoChecks()
        {

        }
    }
}
