using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HiveMind.Events
{
    [CreateAssetMenu(fileName = "UIScriptableObject", menuName = "ScriptableObjects/UI Event")]
    public class UIScriptableObj : ScriptableObject
    {
        #region FIELDS
        [SerializeField] private List<GameObject> shroomCounter;
        [SerializeField] private Vector3 cursorPosition;

        #region EVENTS
        public UnityEvent<Vector2, Vector2, bool> plotTrajectory;
        public UnityEvent deleteLine;
        public UnityEvent<GameObject> addToShroomCounter;
        public UnityEvent<GameObject> removeFromShroomCounter;
        public UnityEvent resetCounterQueue;
        #endregion
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if(plotTrajectory == null)
            {
                plotTrajectory = new UnityEvent<Vector2, Vector2, bool>();
            }

            if(deleteLine == null)
            {
                deleteLine = new UnityEvent();
            }

            if(addToShroomCounter == null)
            {
                addToShroomCounter = new UnityEvent<GameObject>();
            }

            if (removeFromShroomCounter == null)
            {
                removeFromShroomCounter = new UnityEvent<GameObject>();
            }

            if(resetCounterQueue == null)
            {
                resetCounterQueue = new UnityEvent();
            }
            #endregion
        }

        /// <summary>
        /// Set the shroom counter
        /// </summary>
        /// <param name="shroomCounter">The shroom counter</param>
        public void SetShroomCounter(List<GameObject> shroomCounter)
        {
            this.shroomCounter = shroomCounter;
        }

        /// <summary>
        /// Set the cursor position
        /// </summary>
        /// <param name="cursorPosition">The cursor position</param>
        public void SetCursorPosition(Vector3 cursorPosition)
        {
            this.cursorPosition = cursorPosition;
        }

        /// <summary>
        /// Get the shroom counter
        /// </summary>
        /// <returns>The shroom counter</returns>
        public List<GameObject> GetShroomCounter()
        {
            return shroomCounter;
        }

        /// <summary>
        /// Get the cursor position
        /// </summary>
        /// <returns>The cursor position</returns>
        public Vector3 GetCursorPosition()
        {
            return cursorPosition;
        }

        /// <summary>
        /// Plot the Throw UI trajectory
        /// </summary>
        /// <param name="playerPos">The player position</param>
        /// <param name="launchForce">The launch force of the shroom</param>
        /// <param name="facingRight">Whether the player is facing right or not</param>
        public void PlotTrajectory(Vector2 playerPos, Vector2 launchForce, bool facingRight)
        {
            plotTrajectory.Invoke(playerPos, launchForce, facingRight);
        }

        /// <summary>
        /// Delete the Throw UI line
        /// </summary>
        public void DeleteLine()
        {
            deleteLine.Invoke();
        }

        /// <summary>
        /// Add an icon to the shroom counter
        /// </summary>
        /// <param name="shroomToAdd">The icon to add</param>
        public void AddToShroomCounter(GameObject shroomToAdd)
        {
            addToShroomCounter.Invoke(shroomToAdd);
        }

        /// <summary>
        /// Remove an icon from the shroom counter
        /// </summary>
        /// <param name="shroomToRemove">The icon to remove</param>
        public void RemoveFromShroomCounter(GameObject shroomToRemove)
        {
            removeFromShroomCounter.Invoke(shroomToRemove);
        }

        /// <summary>
        /// Reset the counter queue
        /// </summary>
        public void ResetCounterQueue()
        {
            resetCounterQueue.Invoke();
        }

        /// <summary>
        /// Rseet object variables
        /// </summary>
        public void ResetObj()
        {
            shroomCounter = new List<GameObject>();
            cursorPosition = Vector3.zero;
        }
    }
}
