using UnityEngine;
using AK.Wwise;

namespace GoodLuckValley.Audio
{
    public class PlayerSFX : MonoBehaviour
    {
        public float CRAWL = 0.8f;
        public float WALK = 2.0f;
        public float RUN = 3.1f;

        private float maxFallCounters;

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

        [Header("Player Slide")]
        [SerializeField] private bool playingSlide;
        [SerializeField] private AK.Wwise.Event startSlide;
        [SerializeField] private AK.Wwise.Event stopSlide;
        [SerializeField] private AK.Wwise.Event slideImpact;

        [Header("Mushroom Bounce")]
        [SerializeField] private RTPC bounceChainRTPC;
        [SerializeField] private AK.Wwise.Event bounceEvent;

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
        /// Start the slide sound effect
        /// </summary>
        public void StartSlide()
        {
            // Exit case - if already playing the slide sound
            if (playingSlide) return;

            startSlide.Post(gameObject);
            playingSlide = true;
        }

        /// <summary>
        /// Stop the slide sound effect
        /// </summary>
        public void StopSlide()
        {
            // Exit case - if not already playing the slide sound
            if (!playingSlide) return;

            stopSlide.Post(gameObject);
            playingSlide = false;

            slideImpact.Post(gameObject);
        }

        /// <summary>
        /// Play the sound effect for bouncing
        /// </summary>
        /// <param name="bounceCount"></param>
        public void Bounce(int bounceCount)
        {
            // Set the bounce chain RTPC value for sweeteners
            bounceChainRTPC.SetValue(gameObject, bounceCount);

            // Play the bounce event
            bounceEvent.Post(gameObject);
        }

        /// <summary>
        /// Play the sound effect for wall jumping
        /// </summary>
        public void WallJump()
        {
        }
    }
}
