using GoodLuckValley.Audio.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerSFXHandler : MonoBehaviour
    {
        [Header("Sound Data")]
        [SerializeField] private SoundData footsteps;
        [SerializeField] private SoundData land;
        [SerializeField] private SoundData fall;
        [SerializeField] private SoundData bounce;
        [SerializeField] private SoundData wallSlide;
        [SerializeField] private SoundData wallJump;
        [SerializeField] private SoundData jump;

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
        /// Play the sound effect for bouncing
        /// </summary>
        public void Bounce()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(bounce)
                .WithRandomPitch()
                .Play();
        }
    }
}