using UnityEngine;
using AK.Wwise;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Audio
{
    public enum GroundType
    {
        None,
        Grass,
        Dirt,
        Stone
    }

    public class PlayerSFX : MonoBehaviour
    {
        private float maxFallCounters;

        [Header("Layers")]
        [SerializeField] private GroundType groundType;
        [SerializeField] private LayerMask grassLayer;
        [SerializeField] private LayerMask dirtLayer;
        [SerializeField] private LayerMask stoneLayer;

        [Header("Wwise Switches")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;

        [Header("Player Footsteps")]
        [SerializeField] private bool playingGroundImpacts;
        [SerializeField] private RTPC speedRTPC;
        [SerializeField] private AK.Wwise.Event startGroundImpactsEvent;
        [SerializeField] private AK.Wwise.Event stopGroundImpactsEvent;

        [Header("Player Jump")]
        [SerializeField] private AK.Wwise.Event jumpEvent;

        [Header("Player Land")]
        [SerializeField] private AK.Wwise.Event landEvent;

        [Header("Player Fall")]
        [SerializeField] private bool playingFall;
        [SerializeField] private RTPC fallSpeedRTPC;
        [SerializeField] private AK.Wwise.Event startFallEvent;
        [SerializeField] private AK.Wwise.Event stopFallEvent;

        public float CRAWL => 0.8f;
        public float WALK => 2.0f;
        public float RUN => 3.1f;

        public void SetLayer(int layer)
        {
            // Set none as the ground type
            groundType = GroundType.None;

            // Check if any of the layers are hit
            if(grassLayer == (grassLayer | (1 << layer)))
            {
                groundType = GroundType.Grass;
            }
            else if (dirtLayer == (dirtLayer | (1 << layer)))
            {
                groundType = GroundType.Dirt;
            }
            else if (stoneLayer == (stoneLayer | (1 << layer)))
            {
                groundType = GroundType.Stone;
            }

            // Set switches based on the ground type
            switch (groundType)
            {
                case GroundType.Grass:
                    grassSwitch.SetValue(gameObject);
                    break;

                case GroundType.Dirt:
                    dirtSwitch.SetValue(gameObject);
                    break;

                case GroundType.Stone:
                    stoneSwitch.SetValue(gameObject);
                    break;

                case GroundType.None:
                    break;
            }
        }

        /// <summary>
        /// Set the value for the speed RTPC to control player impact sounds
        /// </summary>
        public void SetSpeedRTPC(float value) => speedRTPC.SetValue(gameObject, value);

        // <summary>
        /// Set the value for the fall speed RTPC to control player land sounds
        /// </summary>
        public void SetFallSpeedRTPC(float value)
        {
            // Check if the value is less than 5
            if (value <= 5)
            {
                // Check if the value is exactly 5
                if (value == 5)
                    // Increment the max fall counters
                    maxFallCounters++;

                // Set the final RTPC value
                float rtpcValue = maxFallCounters > 3 ? 5f : 3f;
                fallSpeedRTPC.SetValue(gameObject, rtpcValue);
            }
            else
            {
                // Otherwise, max out the fall speed RTPC value
                fallSpeedRTPC.SetValue(gameObject, 10f);
            }
        }

        /// <summary>
        /// Start playing the sound effects for ground impacts
        /// </summary>
        public void StartGroundImpacts()
        {
            // Exit case - if already playing ground impacts
            if (playingGroundImpacts) return;

            startGroundImpactsEvent.Post(gameObject);
            playingGroundImpacts = true;
        }

        /// <summary>
        /// Stop playing the sound effect for ground impacts
        /// </summary>
        public void StopGroundImpacts()
        {
            // Exit case - if not already playing ground impacts
            if (!playingGroundImpacts) return;

            stopGroundImpactsEvent.Post(gameObject);
            playingGroundImpacts = false;
        }

        /// <summary>
        /// Play the sound effect for jumping
        /// </summary>
        public void Jump() => jumpEvent.Post(gameObject);

        /// <summary>
        /// Play the sound effect for landing
        /// </summary>
        public void Land() => landEvent.Post(gameObject);

        /// <summary>
        /// Reset the max fall counters
        /// </summary>
        public void ResetLandCounter() => maxFallCounters = 0;

        /// <summary>
        /// Start the sound effect for falling
        /// </summary>
        public void StartFall()
        {
            // Exit case - if already playing the fall sound
            if (playingFall) return;

            startFallEvent.Post(gameObject);
            playingFall = true;
        }

        /// <summary>
        /// Stop the falling sound effect
        /// </summary>
        public void StopFall()
        {
            // Exit case - if not already playing the fall sound
            if (!playingFall) return;

            stopFallEvent.Post(gameObject);
            playingFall = false;
        }

        /// <summary>
        /// Play the sound effect for wall jumping
        /// </summary>
        public void WallJump()
        {
        }
    }
}
