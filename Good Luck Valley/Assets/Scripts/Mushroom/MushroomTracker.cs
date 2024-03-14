using GoodLuckValley.Events;
using System.Collections;
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
        [SerializeField] private int limit;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddMushroom(Component sender, object data)
        {
            // Check if the correct data type was sent
            if (data is not SporeData) return;

            // Cast data
            SporeData sporeData = (SporeData)data;

            // Add the spore to the list
            GameObject newSpore = Instantiate(sporeData.Spore, transform.position, Quaternion.identity);

            newSpore.GetComponent<Rigidbody2D>().AddForce(sporeData.LaunchForce);
        }

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

        public void RemoveFirstShroom(Component sender, object data)
        {
            // Destroy the first shroom object
            Destroy(mushrooms[0]);

            // Remove the first shroom from the list
            mushrooms.RemoveAt(0);
        }
    }
}
