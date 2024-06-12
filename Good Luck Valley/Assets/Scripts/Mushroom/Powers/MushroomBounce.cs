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

            public BounceData(Vector2 bounceVector, bool rotated)
            {
                BounceVector = bounceVector;
                Rotated = rotated;
            }
        }

        [Header("Events")]
        [SerializeField] private GameEvent onBounce;
        [Space(10f)]

        [Header("Fields")]
        [SerializeField] private SoundData SFX;
        [SerializeField] private bool isBouncing;
        [SerializeField] private bool canBounce;
        [SerializeField] private float bounceForce;
        

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

            // Create Bounce Data
            BounceData bounceData = new BounceData(
                mushroomData.GetBounceVector(),
                mushroomData.Rotated
            );

            // Raise the bounce event
            // Calls to:
            //  - PlayerController.StartBounce()
            onBounce.Raise(this, bounceData);

            // Play the sound
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(SFX)
                .WithRandomPitch()
                .Play();
        }
    }
}
