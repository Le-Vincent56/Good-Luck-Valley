using GoodLuckValley.World.Tiles;
using UnityEngine;
using AK.Wwise;
using System.Threading;

namespace GoodLuckValley.Audio.SFX
{
    public class PlayerSFXMaster : MonoBehaviour
    {
        public float CRAWL = 0.8f;
        public float WALK = 2.0f;
        public float RUN = 3.1f;

        private float maxFallCounters;

        [Header("Wwise Switches")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;

        [Header("Tile Detection")]
        [SerializeField] private DetectTileType detectTileType;
        private TileType currentTileType;

        [Header("Player Footsteps")]
        [SerializeField] private RTPC speedRTPC;
        [SerializeField] private AK.Wwise.Event startGroundImpactsEvent;
        [SerializeField] private AK.Wwise.Event stopGroundImpactsEvent;

        [Header("Play Land")]
        [SerializeField] private AK.Wwise.Event landEvent;

        [Header("Player Fall")]
        [SerializeField] private RTPC fallSpeedRTPC;
        [SerializeField] private AK.Wwise.Event startFallEvent;
        [SerializeField] private AK.Wwise.Event stopFallEvent;

        [Header("Player Slide")]
        [SerializeField] private AK.Wwise.Event startSlide;
        [SerializeField] private AK.Wwise.Event stopSlide;
        [SerializeField] private AK.Wwise.Event slideImpact;

        private void Awake()
        {
            // Set beginning tile type detection raycast
            detectTileType.RaycastStart = transform.parent.position;
        }

        private void Update()
        {
            // Update the current tile detected
            detectTileType.RaycastStart = transform.parent.position;
            currentTileType = detectTileType.CheckTileType();

            switch (currentTileType)
            {
                case TileType.Grass:
                    grassSwitch.SetValue(gameObject);
                    break;

                case TileType.Dirt:
                    dirtSwitch.SetValue(gameObject);
                    break;

                case TileType.Stone:
                    stoneSwitch.SetValue(gameObject);
                    break;

                case TileType.None:
                    break;
            }
        }

        /// <summary>
        /// Set the value for the speed RTPC to control player impact sounds
        /// </summary>
        /// <param name="value"></param>
        public void SetSpeedRTPC(float value) => speedRTPC.SetValue(gameObject, value);

        /// <summary>
        /// Set the value for the fall speed RTPC to control player land sounds
        /// </summary>
        /// <param name="value"></param>
        public void SetFallSpeedRTPC(float value)
        {
            if(value <= 5)
            {
                float newValue = value;
                if (newValue == 5)
                    maxFallCounters++;

                if(maxFallCounters > 3)
                {
                    fallSpeedRTPC.SetValue(gameObject, 5f);
                } else
                {
                    fallSpeedRTPC.SetValue(gameObject, 3f);
                }
            } else
            {
                fallSpeedRTPC.SetValue(gameObject, 10f);
            }
        }

        /// <summary>
        /// Start playing the sound effects for ground impacts
        /// </summary>
        public void StartGroundImpacts() => startGroundImpactsEvent.Post(gameObject);

        /// <summary>
        /// Stop playing the sound effect for ground impacts
        /// </summary>
        public void StopGroundImpacts() => stopGroundImpactsEvent.Post(gameObject);

        /// <summary>
        /// Play the sound effect for landing
        /// </summary>
        public void Land() => landEvent.Post(gameObject);

        public void ResetLandCounter() => maxFallCounters = 0;

        /// <summary>
        /// Play the sound effecting for falling
        /// </summary>
        public void StartFall() => startFallEvent.Post(gameObject);

        /// <summary>
        /// Stop the falling sound effect
        /// </summary>
        public void StopFall() => stopFallEvent.Post(gameObject);

        /// <summary>
        /// Start the slide sound effect
        /// </summary>
        public void StartSlide() => startSlide.Post(gameObject);

        /// <summary>
        /// Stop the slide sound effect
        /// </summary>
        public void StopSlide()
        {
            stopSlide.Post(gameObject);
            slideImpact.Post(gameObject);
        }

        /// <summary>
        /// Play the sound effect for throwing a spore
        /// </summary>
        public void SporeThrow()
        {
        }

        /// <summary>
        /// Play the sound effect for wall jumping
        /// </summary>
        public void WallJump()
        {
        }
    }
}