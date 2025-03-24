using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class OneshotSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event oneShotEvent;

        /// <summary>
        /// Play the one-shot SFX
        /// </summary>
        public void PlaySFX() => oneShotEvent.Post(gameObject);
    }
}
