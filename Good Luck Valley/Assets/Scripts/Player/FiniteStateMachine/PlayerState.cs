using GoodLuckValley.Player.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerState
    {
        #region FIELDS
        protected PlayerController player;
        protected PlayerStateMachine stateMachine;
        protected PlayerData playerData;

        protected bool isAnimationFinished;

        protected float startTime;

        private string animationBoolName;
        #endregion

        public PlayerState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
        {
            this.player = player;
            this.stateMachine = stateMachine;
            this.playerData = playerData;
            this.animationBoolName = animationBoolName;
        }

        /// <summary>
        /// Enter the PlayerController state
        /// </summary>
        public virtual void Enter()
        {
            // Check states
            DoChecks();

            // Set the animation state
            isAnimationFinished = false;
            player.Anim.SetBool(animationBoolName, true);

            // Set start time
            startTime = Time.time;
        }

        /// <summary>
        /// Exit the PlayerController state
        /// </summary>
        public virtual void Exit()
        {
            // Leave the animation state
            player.Anim.SetBool(animationBoolName, false);
        }
        
        /// <summary>
        /// Update the PlayerController logic
        /// </summary>
        public virtual void LogicUpdate()
        {

        }

        /// <summary>
        /// Update the PlayerController physics
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

        /// <summary>
        /// Trigger something in the middle of an animation
        /// </summary>
        public virtual void AnimationTrigger()
        {

        }

        /// <summary>
        /// Check if the animation is finished
        /// </summary>
        public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
    }
}
