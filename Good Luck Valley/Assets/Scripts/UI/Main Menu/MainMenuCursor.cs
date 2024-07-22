using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.MainMenu
{
    public class MainMenuCursor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CursorElement lastSelectedCursor;
        [SerializeField] private CursorElement startCursor;
        [SerializeField] private CursorElement settingsCursor;
        [SerializeField] private CursorElement quitCursor;

        [Header("Fields")]
        [SerializeField] private bool active;

        private void Awake()
        {
            // Set active to false
            active = false;

            // Initialize the cursors
            startCursor.Init();
            settingsCursor.Init();
            quitCursor.Init();

            // Set the last selected cursor
            lastSelectedCursor = startCursor;
        }

        private void Update()
        {
            // Exit case - if not active
            if (!active) return;

            // Prevent any cases where there would be no cursor selected
            if (lastSelectedCursor != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(lastSelectedCursor.GetButton().gameObject);

            // Set the last selected cursor
            if(startCursor.Selected)
                lastSelectedCursor = startCursor;

            if(settingsCursor.Selected)
                lastSelectedCursor = settingsCursor;

            if(quitCursor.Selected)
                lastSelectedCursor = quitCursor;
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
                EventSystem.current.SetSelectedGameObject(startCursor.GetButton().gameObject);
                startCursor.Select();
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