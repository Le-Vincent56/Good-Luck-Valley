using GoodLuckValley.World.Tiles;
using UnityEngine;
using AK.Wwise;

namespace GoodLuckValley.Audio.SFX
{
    public class FootstepSFX : MonoBehaviour
    {
        [Header("Wwise Fields")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;
        [SerializeField] private AK.Wwise.Event footstepEvent;

        [Header("Fields")]
        [SerializeField] private DetectTileType detectTileType;
        [SerializeField] private float footstepInterval;
        [SerializeField] private float footstepTimer;

        private void Start()
        {
            detectTileType.RaycastStart = transform.parent.position;
        }

        /// <summary>
        /// Handle playing the footsteps sound effects
        /// </summary>
        public void Footsteps()
        {
            // Increment the foot step timer
            footstepTimer += Time.deltaTime;

            // Check if the footstep timer has reached the footstep interval
            if (footstepTimer >= footstepInterval)
            {
                TileType tileType = detectTileType.CheckTileType();

                switch (tileType)
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

                footstepEvent.Post(gameObject);

                // Reset the footstep timer
                ResetFootsteps();
            }
        }

        /// <summary>
        /// Reset the timer for footsteps
        /// </summary>
        public void ResetFootsteps() => footstepTimer = 0;
    }
}