using GoodLuckValley.Audio.Sound;
using GoodLuckValley.World.Tiles;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerSFXHandler : MonoBehaviour
    {
        [Header("Sound Data - Movement")]
        [SerializeField] private SoundData dirtFootsteps;
        [SerializeField] private SoundData grassFootsteps;
        [SerializeField] private SoundData land;
        [SerializeField] private SoundData fall;
        [SerializeField] private SoundData wallSlide;
        [SerializeField] private SoundData wallJump;
        [SerializeField] private SoundData jump;

        [Header("Sound Data - Mushroom")]
        [SerializeField] private SoundData sporeThrow;

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
            if(footstepTimer >= footstepInterval)
            {
                TileType tileType = detectTileType.CheckTileType();

                switch (tileType)
                {
                    case TileType.Grass:
                        SoundManager.Instance.CreateSoundBuilder()
                            .WithSoundData(grassFootsteps)
                            .WithRandomPitch()
                            .Play();
                        break;

                    case TileType.Dirt:
                        SoundManager.Instance.CreateSoundBuilder()
                            .WithSoundData(dirtFootsteps)
                            .WithRandomPitch()
                            .Play();
                        break;

                    case TileType.None:
                        break;
                }

                // Reset the footstep timer
                ResetFootsteps();
            }
        }

        /// <summary>
        /// Reset the timer for footsteps
        /// </summary>
        public void ResetFootsteps() => footstepTimer = 0;
        
        /// <summary>
        /// Play the sound effect for landing
        /// </summary>
        public void Land()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(land)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Play the sound effecting for falling
        /// </summary>
        public void Fall()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(fall)
                .Play();
        }

        /// <summary>
        /// Stop the falling sound effect
        /// </summary>
        public void StopFall() => SoundManager.Instance.StopAllSoundsOfType(fall);

        /// <summary>
        /// Play the sound effect for throwing a spore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SporeThrow(Component sender, object data)
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(sporeThrow)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Play the sound effect for wall jumping
        /// </summary>
        public void WallJump()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(wallJump)
                .WithRandomPitch()
                .Play();
        }
    }
}