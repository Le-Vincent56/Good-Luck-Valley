using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus
{
    public class MenuCursor : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private bool active;
        [SerializeField] private CursorElement lastSelectedCursor;
        [SerializeField] private List<CursorElement> cursorElements = new List<CursorElement>();

        public bool Active { get => active; }

        private void Awake()
        {
            // Set active to false
            active = false;

            foreach (CursorElement cursor in cursorElements)
            {
                cursor.Init(this);
            }
        }

        private void Update()
        {
            // Exit case - if not active
            if (!active) return;

            // Exit case - if the Event system is null or is not enabled
            if (EventSystem.current == null || !EventSystem.current.enabled) return;

            // Prevent any cases where there would be no cursor selected
            if (lastSelectedCursor != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(lastSelectedCursor.GetSelectable().gameObject);

            // Loop through every cursor
            foreach (CursorElement cursor in cursorElements)
            {
                // Check if the cursor is selected
                if (cursor.Selected)
                {
                    // If so, set the last selected cursor to that cursor
                    lastSelectedCursor = cursor;
                    EventSystem.current.SetSelectedGameObject(cursor.GetSelectable().gameObject);
                } else
                {
                    // Deactivate any non-selected cursors
                    cursor.Deactivate();
                }
            }
        }

        /// <summary>
        /// Activate the Main Menu cursors
        /// </summary>
        public void ShowCursors()
        {
            // Deactivate the other cursors
            foreach(CursorElement cursor in cursorElements)
            {
                cursor.Deactivate();
            }

            // Select the last selected cursor
            if (lastSelectedCursor != null)
            {
                // Activate the cursor's object
                lastSelectedCursor.Activate();
            }
            // If it's null, select the start cursor
            else
            {
                // Activate the cursor's object
                cursorElements[0].Activate();
            }
        }

        public void ActivateCursors()
        {
            // Set active to true
            active = true;

            // Select the last selected cursor
            if (lastSelectedCursor != null)
            {
                // Set the selected game object for the event system
                EventSystem.current.SetSelectedGameObject(lastSelectedCursor.GetSelectable().gameObject);
            }
            // If it's null, select the start cursor
            else
            {
                // Set the selected game object for the event system
                EventSystem.current.SetSelectedGameObject(cursorElements[0].GetSelectable().gameObject);
            }
        }

        /// <summary>
        /// Deactivate the Main Menu cursors
        /// </summary>
        public void DeactivateCursors()
        {
            // Nullify the event system selected game object
            EventSystem.current.SetSelectedGameObject(null);

            // Set active to false
            active = false;
        }
    }
}