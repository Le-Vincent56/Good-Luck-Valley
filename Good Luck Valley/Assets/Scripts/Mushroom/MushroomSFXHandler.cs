using GoodLuckValley.Audio.Sound;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomSFXHandler : MonoBehaviour
    {
        [Header("Sound Data")]
        [SerializeField] private SoundData grow;
        [SerializeField] private SoundData bounce;

        /// <summary>
        /// Play the sound effect for growing
        /// </summary>
        public void Grow()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(grow)
                .WithPosition((Vector2)transform.position)
                .WithRandomPitch()
                .Play();
        }

        /// <summary>
        /// Play the sound effect for bouncing
        /// </summary>
        public void Bounce()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(bounce)
                .WithPosition((Vector2)transform.position)
                .WithRandomPitch()
                .Play();
        }
    }
}