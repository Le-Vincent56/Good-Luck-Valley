using GoodLuckValley.World.Tiles;
using UnityEngine;
using AK.Wwise;

namespace GoodLuckValley.Audio.SFX
{
    public class PlayerSFXMaster : MonoBehaviour
    {
        [Header("Wwise")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;

        [Header("Tile Detection")]
        [SerializeField] private DetectTileType detectTileType;
        private TileType currentTileType;

        [Header("Player Footsteps")]
        [SerializeField] private AK.Wwise.Event footstepEvent;
        [SerializeField] private float footstepInterval;
        [SerializeField] private float footstepTimer;


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

        public void Footsteps()
        {
            // Increment the foot step timer
            footstepTimer += Time.deltaTime;

            // Check if the footstep timer has reached the footstep interval
            if (footstepTimer >= footstepInterval)
            {
                footstepEvent.Post(gameObject);

                // Reset the footstep timer
                ResetFootsteps();
            }
        }
        public void ResetFootsteps() => footstepTimer = 0;

        /// <summary>
        /// Play the sound effect for landing
        /// </summary>
        public void Land()
        {
        }

        /// <summary>
        /// Play the sound effecting for falling
        /// </summary>
        public void Fall()
        {
        }

        /// <summary>
        /// Stop the falling sound effect
        /// </summary>
        public void StopFall()
        {
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
    }
}