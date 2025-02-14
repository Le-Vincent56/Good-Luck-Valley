using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class OneshotSFX : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event oneShotEvent;

        public void PlaySFX()
        {
            Debug.Log($"Playing SFX: {oneShotEvent}");
            oneShotEvent.Post(gameObject);
        }
    }
}
