using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class MushroomSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event bounceEvent;
        [SerializeField] private AK.Wwise.Event dissipateEvent;

        /// <summary>
        /// Play the sound effect for bouncing
        /// </summary>
        public void Bounce() => bounceEvent.Post(gameObject);

        /// <summary>
        /// Play the sound effect for dissipating
        /// </summary>
        public void Dissipate() => dissipateEvent.Post(gameObject);
    }
}
