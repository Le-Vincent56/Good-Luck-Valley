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
            mushroomList[0].GetComponent<MushroomController>().Dissipate();

            // Remove the first shroom from the list
            mushroomList.RemoveAt(0);

            // Re-arrange indexes
            foreach (GameObject shroom in mushrooms)
            {
                shroom.GetComponent<MushroomInfo>().SetIndex(mushrooms.IndexOf(shroom));
            }
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

            // Set the index of the mushroom
            MushroomInfo info = mushroom.GetComponent<MushroomInfo>();
            info.SetIndex(mushrooms.IndexOf(mushroom));
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

            // Decompose the mushroom at that index
            // and remove it from the list
            mushrooms[index].GetComponent<MushroomController>().Dissipate();
            mushrooms.RemoveAt(index);

            // Re-arrange indexes
            foreach(GameObject shroom in mushrooms)
            {
                shroom.GetComponent<MushroomInfo>().SetIndex(mushrooms.IndexOf(shroom));
            }
        }

        /// <summary>
        /// Recall the most recently thrown Mushroom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void RecallLast(Component sender, object data)
        {
            // Check if there are any mushrooms to recall
            if (mushrooms.Count < 1) return;

            // Decompose the last mushroom
            mushrooms[mushrooms.Count - 1].GetComponent<MushroomController>().Dissipate();
            mushrooms.RemoveAt(mushrooms.Count - 1);

            // Re-arrange indexes
            foreach (GameObject shroom in mushrooms)
            {
                shroom.GetComponent<MushroomInfo>().SetIndex(mushrooms.IndexOf(shroom));
            }
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
                if (mushroom != null)
                    // Decompose each mushroom
                    mushroom.GetComponent<MushroomController>().Dissipate();
            }

            // Clear the list
            mushrooms.Clear();
        }
    }
}
