using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom.StateMachine
{
    public class MushroomState
    {
        #region FIELDS
        protected MushroomController mushroom;
        protected MushroomStateMachine stateMachine;

        protected bool isAnimationFinished;
        protected float startTime;
        private string animationBoolName;
        #endregion

        public MushroomState(MushroomController mushroom, MushroomStateMachine stateMachine, string animationBoolName)
        {
            this.mushroom = mushroom;
            this.stateMachine = stateMachine;
            this.animationBoolName = animationBoolName;
        }

        /// <summary>
        /// Enter the State
        /// </summary>
        public virtual void Enter()
        {
            // Check states
            DoChecks();

            // Set the animation state
            isAnimationFinished = false;
            mushroom.Anim.SetBool(animationBoolName, true);

            // Set start time
            startTime = Time.time;
        }

        /// <summary>
        /// Exit the State
        /// </summary>
        public virtual void Exit()
        {
            // Leave the animation state
            mushroom.Anim.SetBool(animationBoolName, false);
        }

        /// <summary>
        /// Update the State logic
        /// </summary>
        public virtual void LogicUpdate()
        {

        }

        /// <summary>
        /// Update the State physics
        /// </summary>
        public virtual void PhysicsUpdate()
        {
            // Check states
            DoChecks();
        }

        /// <summary>
        /// Run certain State checks
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
