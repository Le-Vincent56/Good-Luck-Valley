using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomBounce : MonoBehaviour
    {
        public struct BounceData
        {
            public Vector2 BounceVector;
            public ForceMode2D ForceMode;

            public BounceData(Vector2 bounceVector, ForceMode2D forceMode)
            {
                BounceVector = bounceVector;
                ForceMode = forceMode;
            }
        }

        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onBounce;
        [Space(10f)]
        #endregion

        #region FIELDS
        [SerializeField] private bool canBounce;
        [SerializeField] private float bounceForce;
        #endregion

        /// <summary>
        /// Collect the Mushroom's Bounce Data and send it to the Player to Bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Bounce(Component sender, object data)
        {
            // Check if the correct data type was sent
            if (data is not MushroomData) return;

            // Cast the data
            MushroomData mushroomData = (MushroomData)data;

            // Create Bounce Data
            BounceData bounceData = new BounceData(mushroomData.GetBounceVector(), ForceMode2D.Impulse);

            // Raise the bounce event
            // Calls to:
            //  - PlayerController.StartBounce()
            onBounce.Raise(this, bounceData);
        }
    }
}
