using GoodLuckValley.Audio.Sound;
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
            public bool Rotated;
            public int BounceCount;

            public BounceData(Vector2 bounceVector, bool rotated, int bounceCount)
            {
                BounceVector = bounceVector;
                Rotated = rotated;
                BounceCount = bounceCount;
            }
        }

        [Header("Events")]
        [SerializeField] private GameEvent onBounce;
        [Space(10f)]

        [Header("Fields")]
        [SerializeField] private int bounceCount;
        [SerializeField] private bool isBouncing;
        [SerializeField] private bool canBounce;
        [SerializeField] private float bounceForce;

        private void Start()
        {
            bounceCount = 0;
        }


        /// <summary>
        /// Collect the Mushroom's Bounce Data and send it to the Player to Bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Bounce(Component sender, object data)
        {
            // Check if the correct data type was sent
            if (data is not MushroomInfo || isBouncing) return;

            // Cast the data
            MushroomInfo mushroomData = (MushroomInfo)data;

            switch (mushroomData.Type)
            {
                case ShroomType.Regular:
                    if (bounceCount < 3)
                        bounceCount++;
                    else if(bounceCount >= 3)
                        bounceCount = 1;
                    break;

                case ShroomType.Quick:
                    Debug.Log("Quick!");
                    bounceCount = 1;
                    break;

                case ShroomType.Wall:
                    break;
            }

            // Create Bounce Data
            BounceData bounceData = new BounceData(
                mushroomData.GetBounceVector(),
                mushroomData.Rotated,
                bounceCount
            );

            // Raise the bounce event
            // Calls to:
            //  - PlayerController.StartBounce()
            onBounce.Raise(this, bounceData);
        }

        public void ResetBounce(Component sender, object data)
        {
            // Reset the bounce count
            bounceCount = 0;
        }
    }
}
