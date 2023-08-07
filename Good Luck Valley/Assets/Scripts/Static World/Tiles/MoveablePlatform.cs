using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.Tiles
{
    public abstract class MoveablePlatform : MonoBehaviour
    {
        #region FIELDS
        protected bool isTriggered;
        protected List<GameObject> stuckShrooms = new List<GameObject>();
        #endregion

        #region PROPERTIES
        public bool IsTriggered { get { return isTriggered; } set { isTriggered = value; } }
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (isTriggered)
            {
                Move();
            }
        }

        /// <summary>
        /// Move the Platform
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// Checks if the required amount of shroom to move the platform is on it
        /// </summary>
        /// <param name="shrooms"></param>
        public void CheckWeight(GameObject shroom)
        {
            stuckShrooms.Add(shroom);
            isTriggered = true;
        }
    }
}