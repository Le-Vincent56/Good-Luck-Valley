using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.MainMenu
{
    public class StartMenuCursor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CursorElement lastSelectedCursor;
        [SerializeField] private CursorElement startSlot1Cursor;
        [SerializeField] private CursorElement startSlot2Cursor;
        [SerializeField] private CursorElement startSlot3Cursor;
        [SerializeField] private CursorElement deleteSlot1Cursor;
        [SerializeField] private CursorElement deleteSlot2Cursor;
        [SerializeField] private CursorElement deleteSlot3Cursor;
        [SerializeField] private CursorElement backCursor;

        [Header("Fields")]
        [SerializeField] private bool active;

        private void Awake()
        {
            // Set active to false
            active = false;

            // Initialize the cursors
            startSlot1Cursor.Init();
            startSlot2Cursor.Init();
            startSlot3Cursor.Init();
            deleteSlot1Cursor.Init();
            deleteSlot2Cursor.Init();
            deleteSlot3Cursor.Init();
            backCursor.Init();

            // Set the last selected cursor
            lastSelectedCursor = startSlot1Cursor;
        }

        private void Update()
        {
            // Exit case - if not active
            if (!active) return;

            // Prevent any cases where there would be no cursor selected
            if (lastSelectedCursor != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(lastSelectedCursor.GetButton().gameObject);

            // Set the last selected cursor
            if (startSlot1Cursor.Selected)
                lastSelectedCursor = startSlot1Cursor;

            if (startSlot2Cursor.Selected)
                lastSelectedCursor = startSlot2Cursor;

            if (startSlot3Cursor.Selected)
                lastSelectedCursor = startSlot3Cursor;

            if (deleteSlot1Cursor.Selected)
                lastSelectedCursor = deleteSlot1Cursor;

            if (deleteSlot2Cursor.Selected)
                lastSelectedCursor = deleteSlot2Cursor;

            if (deleteSlot3Cursor.Selected)
                lastSelectedCursor = deleteSlot3Cursor;

            if (backCursor.Selected)
                lastSelectedCursor = backCursor;
        }

        /// <summary>
        /// Activate the Main Menu cursors
        /// </summary>
        public void ActivateCursors()
        {
            // Set active to true
            active = true;

            // Select the last selected cursor
            if (lastSelectedCursor != null)
            {
                // Set the selected game object
                EventSystem.current.SetSelectedGameObject(lastSelectedCursor.GetButton().gameObject);
                lastSelectedCursor.Select();
            }
            // If it's null, select the start cursor
            else
            {
                // Set the selected game object
                EventSystem.current.SetSelectedGameObject(startSlot1Cursor.GetButton().gameObject);
                startSlot1Cursor.Select();
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