using GoodLuckValley.Audio.Sound;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerSFXHandler : MonoBehaviour
    {
        [Header("Sound Data - Movement")]
        [SerializeField] private SoundData footsteps;
        [SerializeField] private SoundData land;
        [SerializeField] private SoundData fall;
        [SerializeField] private SoundData wallSlide;
        [SerializeField] private SoundData wallJump;
        [SerializeField] private SoundData jump;

        [Header("Sound Data - Mushroom")]
        [SerializeField] private SoundData sporeThrow;

        [Header("Fields")]
        [SerializeField] private float footstepInterval;
        [SerializeField] private float footstepTimer;

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
                SoundManager.Instance.CreateSoundBuilder()
                    .WithSoundData(footsteps)
                    .WithRandomPitch()
                    .Play();

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