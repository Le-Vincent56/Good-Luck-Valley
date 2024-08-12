using GoodLuckValley.Events;
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
            public int Index;
            public GameObject Mushroom;

            public BounceData(Vector2 bounceVector, bool rotated, int bounceCount, int index, GameObject mushroom)
            {
                BounceVector = bounceVector;
                Rotated = rotated;
                BounceCount = bounceCount;
                Index = index;
                Mushroom = mushroom;
            }
        }

        [Header("Events")]
        [SerializeField] private GameEvent onBounce;
        [Space(10f)]

        [Header("Fields")]
        [SerializeField] private int bounceCount;
        [SerializeField] private bool isBouncing;
        [SerializeField] private float bounceTimer;
        [SerializeField] private float nextBounceBuffer;
        [SerializeField] private bool canBounce;
        [SerializeField] private float bounceForce;

        private void Start()
        {
            bounceCount = 0;
        }

        private void Update()
        {
            // Return if not bouncing
            if (!isBouncing) return;

            // Update the bounce buffer
            if (bounceTimer < nextBounceBuffer)
                bounceTimer += Time.deltaTime;
            else
                isBouncing = false;
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
                    // Increment bounces
                    bounceCount++;
                    break;

                case ShroomType.Quick:
                    // Don't do anything, as it should preserve the bounce count
                    break;

                case ShroomType.Wall:
                    break;
            }

            // Create Bounce Data
            BounceData bounceData = new BounceData(
                mushroomData.GetBounceVector(),
                mushroomData.Rotated,
                bounceCount,
                mushroomData.Index,
                mushroomData.gameObject
            );

            // Set bounce variables
            isBouncing = true;
            bounceTimer = 0;

            // Raise the bounce event
            // Calls to:
            //  - PlayerController.Bounce()
            //  - MsuhroombounceEffect.ApplyEffect()
            onBounce.Raise(this, bounceData);
        }

        public void ResetBounceCount(Component sender, object data)
        {
            // Reset the bounce count
            bounceCount = 0;
        }
    }
}
