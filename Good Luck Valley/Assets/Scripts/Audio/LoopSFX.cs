using UnityEngine;

namespace GoodLuckValley
{
    public class LoopSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event playEvent;
        [SerializeField] private AK.Wwise.Event stopEvent;

        /// <summary>
        /// Play the looping SFX
        /// </summary>
        public void PlaySFX() => playEvent.Post(gameObject);

        /// <summary>
        /// Stop the looping SFX
        /// </summary>
        public void StopSFX() => stopEvent.Post(gameObject);
    }
}
