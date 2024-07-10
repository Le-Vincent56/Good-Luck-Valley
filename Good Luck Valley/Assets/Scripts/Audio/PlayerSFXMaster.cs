using GoodLuckValley.Audio.Sound;
using UnityEngine;

namespace GoodLuckValley.Audio.SFX
{
    public class PlayerSFXMaster : MonoBehaviour
    {
        private FootstepSFX footstepSFX;

        private void Awake()
        {
            footstepSFX = GetComponentInChildren<FootstepSFX>();
        }

        public void Footsteps() => footstepSFX.Footsteps();
        public void ResetFootsteps() => footstepSFX.ResetFootsteps();

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
    }
}