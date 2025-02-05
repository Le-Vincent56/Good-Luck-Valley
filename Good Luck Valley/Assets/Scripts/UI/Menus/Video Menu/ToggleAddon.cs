using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class ToggleAddon : MonoBehaviour, ISelectHandler
    {
        [Header("References")]
        [SerializeField] private Toggle toggle;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event hoverEvent;
        [SerializeField] private AK.Wwise.Event toggleOnEvent;
        [SerializeField] private AK.Wwise.Event toggleOffEvent;

        private void Awake()
        {
            // Get components
            toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool active)
        {
            // Check if active
            if(active)
            {
                // Play the toggle on event
                toggleOnEvent.Post(gameObject);

                return;
            }

            // Play the toggle off event
            toggleOffEvent.Post(gameObject);
        }

        public void OnSelect(BaseEventData eventData)
        {
            // Play the hover event
            hoverEvent.Post(gameObject);
        }
    }
}
