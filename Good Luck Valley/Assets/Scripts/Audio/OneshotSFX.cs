using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class OneshotSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event oneShotEvent;

        public void PlaySFX() => oneShotEvent.Post(gameObject);
    }
}
