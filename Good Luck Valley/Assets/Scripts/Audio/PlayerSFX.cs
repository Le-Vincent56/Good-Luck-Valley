using UnityEngine;
using AK.Wwise;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;

namespace GoodLuckValley.Audio
{
    public class PlayerSFX : MonoBehaviour
    {
        private LayerDetection layerDetection;

        private float maxFallCounters;

        [Header("Wwise Switches")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;

        [Header("Footsteps")]
        [SerializeField] private bool playingGroundImpacts;
        [SerializeField] private RTPC speedRTPC;
        [SerializeField] private AK.Wwise.Event startGroundImpactsEvent;
        [SerializeField] private AK.Wwise.Event stopGroundImpactsEvent;

        [Header("Jump")]
        [SerializeField] private AK.Wwise.Event jumpEvent;
        [SerializeField] private AK.Wwise.Event doubleJumpEvent;

        [Header("Land")]
        [SerializeField] private AK.Wwise.Event landEvent;

        [Header("Fall")]
        [SerializeField] private bool playingFall;
        [SerializeField] private RTPC fallSpeedRTPC;
        [SerializeField] private AK.Wwise.Event startFallEvent;
        [SerializeField] private AK.Wwise.Event stopFallEvent;

        [Header("Wallslide")]
        [SerializeField] private bool playingWallSlide;
        [SerializeField] private AK.Wwise.Event startWallSlideEvent;
        [SerializeField] private AK.Wwise.Event stopWallSlideEvent;
        [SerializeField] private AK.Wwise.Event wallSlideEndEvent;

        [Header("Fireflies")]
        [SerializeField] private Switch firefliesSwitch;
        [SerializeField] private Switch noFirefliesSwitch;
        [SerializeField] private AK.Wwise.Event feedFirefliesEvent;
        [SerializeField] private AK.Wwise.Event pickFireflyFruitEvent;

        public float CRAWL => 0.8f;
        public float WALK => 2.0f;
        public float RUN => 3.1f;

        private EventBinding<SetPaused> onSetPaused;

        private void Awake()
        {
            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();
        }

        private void OnEnable()
        {
            layerDetection.OnGroundLayerChange += SetSFXGroundLayer;
            layerDetection.OnWallTypeChange += SetSFXWallLayer;

            onSetPaused = new EventBinding<SetPaused>(PauseSounds);
            EventBus<SetPaused>.Register(onSetPaused);
        }

        private void OnDisable()
        {
            layerDetection.OnGroundLayerChange -= SetSFXGroundLayer;
            layerDetection.OnWallTypeChange -= SetSFXWallLayer;

            EventBus<SetPaused>.Deregister(onSetPaused);
        }

        private void OnDestroy()
        {
            StopGroundImpacts();
            StopFall();
            StopWallSlide();
        }

        private void PauseSounds(SetPaused eventData)
        {
            if (eventData.Paused)
            {
                StopGroundImpacts(true);
                StopFall(true);
                StopWallSlide(true);

                return;
            }

            if (playingGroundImpacts)
                StartGroundImpacts();

            if (playingFall)
                StartFall();

            if (playingWallSlide)
                StartWallSlide();
        }

        /// <summary>
        /// Set the SFX Layer for ground changes
        /// </summary>
        public void SetSFXGroundLayer(GroundType groundType)
        {
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
        /// Set the SFX Layer for wall changes
        /// </summary>
        public void SetSFXWallLayer(WallType wallType)
        {
            // Set switches based on the ground type
            switch (wallType)
            {
                case WallType.Grass:
                    grassSwitch.SetValue(gameObject);
                    break;

                case WallType.Dirt:
                    dirtSwitch.SetValue(gameObject);
                    break;

                case WallType.Stone:
                    stoneSwitch.SetValue(gameObject);
                    break;

                case WallType.None:
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
        public void StopGroundImpacts(bool soft = false)
        {
            // Exit case - if not already playing ground impacts
            if (!playingGroundImpacts) return;

            stopGroundImpactsEvent.Post(gameObject);

            // Exit case - if a soft stop
            if (soft) return;

            playingGroundImpacts = false;
        }

        /// <summary>
        /// Play the sound effect for jumping
        /// </summary>
        public void Jump() => jumpEvent.Post(gameObject);

        /// <summary>
        /// Play the sound effect for double jumping
        /// </summary>
        public void DoubleJump() => doubleJumpEvent.Post(gameObject);

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
        public void StopFall(bool soft = false)
        {
            // Exit case - if not already playing the fall sound
            if (!playingFall) return;

            stopFallEvent.Post(gameObject);

            // Exit case - if a soft start
            if (soft) return;

            playingFall = false;
        }

        /// <summary>
        /// Start the wall sliding sound effect
        /// </summary>
        public void StartWallSlide()
        {
            // Exit case - if already playing the wall slide sound
            if(playingWallSlide) return;

            startWallSlideEvent.Post(gameObject);
            playingWallSlide = true;
        }

        /// <summary>
        /// Stop the wall sliding sound effect
        /// </summary>
        public void StopWallSlide(bool soft = false)
        {
            // Exit case - if not already playing the wall slide sound
            if (!playingWallSlide) return;

            stopWallSlideEvent.Post(gameObject);

            // Exit case - if a soft stop
            if (soft) return;

            playingWallSlide = false;
        }

        /// <summary>
        /// Play the wall slide end impact sound effect
        /// </summary>
        public void PlayWallSlideEnd() => wallSlideEndEvent.Post(gameObject);

        /// <summary>
        /// Add the fireflies ambience sounds
        /// </summary>
        public void AddFireflies() => firefliesSwitch.SetValue(AmbienceManager.Instance.gameObject);

        /// <summary>
        /// Remove the fireflies ambience sounds
        /// </summary>
        public void RemoveFireflies() => noFirefliesSwitch.SetValue(AmbienceManager.Instance.gameObject);

        /// <summary>
        /// Play the SFX for feeding fireflies
        /// </summary>
        public void FeedFireflies() => feedFirefliesEvent.Post(gameObject);

        /// <summary>
        /// Play the SFX for picking firefly fruit
        /// </summary>
        public void PickFireflyFruit() => pickFireflyFruitEvent.Post(gameObject);
    }
}
