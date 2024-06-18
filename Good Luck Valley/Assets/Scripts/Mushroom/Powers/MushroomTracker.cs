using GoodLuckValley.Events;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomTracker : MonoBehaviour
    {
        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onSendCountToLimit;
        #endregion

        #region FIELDS
        [Header("Details")]
        [SerializeField] private List<GameObject> mushrooms;
        [SerializeField] private int limit = 3;
        #endregion

        /// <summary>
        /// Check the current Mushroom list's count against the limit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void CheckCountToLimit(Component sender, object data)
        {
            bool atOrOverLimit;

            // Check if the mushrooms are above, below, or at their limit
            if(mushrooms.Count < limit)
                atOrOverLimit = false;
            else atOrOverLimit = true;

            // Send data
            // Calls to:
            //  - MushroomThrow.HandleThrow()
            onSendCountToLimit.Raise(this, atOrOverLimit); 
        }

        /// <summary>
        /// Remove the first shroom in a list list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void RemoveFirstShroom(List<GameObject> mushroomList)
        {
            // Destroy the first shroom object
            Destroy(mushroomList[0]);

            // Remove the first shroom from the list
            mushroomList.RemoveAt(0);
        }

        /// <summary>
        /// Add a Mushroom to the Mushroom list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void AddMushroom(Component sender, object data)
        {
            // Check the correct data was sent
            if (data is not GameObject) return;

            // Cast the data
            GameObject mushroom = (GameObject)data;

            // Remove the first shroom if necessary
            if(mushrooms.Count >= limit)
            {
                RemoveFirstShroom(mushrooms);
            }

            // Add the mushroom to the list
            mushrooms.Add(mushroom);
        }

        /// <summary>
        /// Remove a shroom from the Mushroom list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void RemoveShroom(Component sender, object data)
        {
            // Check the correct data was sent
            if (data is not GameObject) return;

            // Cast the data
            GameObject mushroom = (GameObject)data;

            // Get the index of the mushroom
            int index = mushrooms.IndexOf(mushroom);

            // Destroy the mushroom at that index
            // and remove it from the list
            Destroy(mushrooms[index]);
            mushrooms.RemoveAt(index);
        }

        /// <summary>
        /// Recall the most recently thrown Mushroom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void RecallLast(Component sender, object data)
        {
            // Destroy the last mushroom and remove it from the list
            Destroy(mushrooms[mushrooms.Count - 1]);
            mushrooms.RemoveAt(mushrooms.Count - 1);
        }

        /// <summary>
        /// Recall all thrown mushrooms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void RecallAll(Component sender, object data)
        {
            // Iterate through the mushroom list
            foreach(GameObject mushroom in mushrooms)
            {
                // Destroy the mushroom
                if (mushroom != null) Destroy(mushroom);
            }

            // Clear the list
            mushrooms.Clear();
        }
    }
}
