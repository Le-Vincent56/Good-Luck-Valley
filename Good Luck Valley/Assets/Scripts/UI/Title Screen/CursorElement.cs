using GoodLuckValley.Extensions;
using GoodLuckValley.UI.Menus;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class CursorElement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Selectable selectable;
        [SerializeField] private GameObject cursor;

        [Header("Fields")]
        [SerializeField] private bool selected;
        [SerializeField] private List<Image> images;

        public bool Selected { get => selected; set => selected = value; }

        /// <summary>
        /// Initialize the Cursor
        /// </summary>
        /// <param name="manager"></param>
        public void Init(MenuCursor manager)
        {
            // Set variables
            selected = false;

            // Set event triggers
            AddEventTrigger(selectable.gameObject, EventTriggerType.Select, OnSelect);
            AddEventTrigger(selectable.gameObject, EventTriggerType.PointerEnter, OnSelect);
            AddEventTrigger(selectable.gameObject, EventTriggerType.Deselect, OnDeselect);

            // Verify Image component(s)
            if(cursor.GetComponent<Image>() != null)
            {
                images.Add(cursor.GetComponent<Image>());
            } else
            {
                images = GetComponentsInChildren<Image>().ToList();
            }
        }

        /// <summary>
        /// Add an event trigger to the Cursor
        /// </summary>
        private void AddEventTrigger(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            // Get or add an event trigger from the target component
            EventTrigger eventTrigger = target.GetOrAdd<EventTrigger>();

            // Create an entry object and associate it with the given event type
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;

            // Add the callback
            entry.callback.AddListener(callback);

            // Add the entry to the triggers
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// Handle Cursor functionality on select
        /// </summary>
        /// <param name="eventData"></param>
        private void OnSelect(BaseEventData eventData)
        {
            // Exit case - if the selectable is not interactable
            if (!selectable.interactable) return;

            Activate();
        }

        /// <summary>
        /// Handle Cursor functionality on deselect
        /// </summary>
        /// <param name="eventData"></param>
        private void OnDeselect(BaseEventData eventData) => Deactivate();

        /// <summary>
        /// Activate the Cursor
        /// </summary>
        public void Activate()
        {
            foreach(Image image in images)
            {
                image.enabled = true;
            }

            cursor.SetActive(true);
            Selected = true;
        }

        /// <summary>
        /// Deactivate the Cursor
        /// </summary>
        public void Deactivate()
        {
            foreach(Image image in images)
            {
                image.enabled = false;

                cursor.SetActive(false);
                Selected = false;
            }
        }

        /// <summary>
        /// Get the associated Selectable
        /// </summary>
        /// <returns></returns>
        public Selectable GetSelectable() => selectable;
    }
}