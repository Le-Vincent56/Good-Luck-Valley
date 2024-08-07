using GoodLuckValley.Extensions;
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

        public void Init()
        {
            selected = false;

            AddEventTrigger(selectable.gameObject, EventTriggerType.Select, OnSelect);
            AddEventTrigger(selectable.gameObject, EventTriggerType.PointerEnter, OnSelect);
            AddEventTrigger(selectable.gameObject, EventTriggerType.Deselect, OnDeselect);

            if(cursor.GetComponent<Image>() != null)
            {
                images.Add(cursor.GetComponent<Image>());
            } else
            {
                images = GetComponentsInChildren<Image>().ToList();
            }
        }

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

        private void OnSelect(BaseEventData eventData) => Activate();

        private void OnDeselect(BaseEventData eventData) => Deactivate();

        public void Activate()
        {
            foreach(Image image in images)
            {
                image.enabled = true;
            }

            cursor.SetActive(true);
            Selected = true;
        }

        public void Deactivate()
        {
            foreach(Image image in images)
            {
                image.enabled = false;

                cursor.SetActive(false);
                Selected = false;
            }
        }

        public Selectable GetSelectable() => selectable;
    }
}