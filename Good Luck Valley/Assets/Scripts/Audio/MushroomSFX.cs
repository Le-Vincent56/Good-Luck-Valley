using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class MushroomSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event bounceEvent;

        /// <summary>
        /// Play the sound effect for bouncing
        /// </summary>
        public void Bounce() => bounceEvent.Post(gameObject);
    }
}
