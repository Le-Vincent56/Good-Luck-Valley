using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class DropdownAddon : MonoBehaviour, ISelectHandler
    {
        [Header("References")]
        [SerializeField] private Dropdown dropdown;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event hoverEvent;
        [SerializeField] private AK.Wwise.Event selectEvent;

        private void Awake()
        {
            // Get components
            dropdown = GetComponent<Dropdown>();
        }

        private void OnEnable()
        {
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(int index)
        {
            // Post the select event
            selectEvent.Post(gameObject);
        }

        public void OnSelect(BaseEventData eventData)
        {
            // Post the hover event
            hoverEvent.Post(gameObject);
        }
    }
}
