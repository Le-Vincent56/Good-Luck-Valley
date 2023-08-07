using UnityEngine;
using UnityEngine.InputSystem;

namespace HiveMind.Menus
{
    public class MenuCursor : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] Camera cam;
        #endregion

        #region FIELDS
        private Vector2 cursorPosition;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;

            // Get components
            cam = Camera.main;

            // Cursor fields
            cursorPosition = transform.position;

            // Confine the cursor to the window
            Cursor.lockState = CursorLockMode.Confined;

        }

        // Update is called once per frame
        void Update()
        {
            Cursor.visible = false;

            // If using mouse, track mouse
            cursorPosition = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y));

            // Draw cursor
            transform.position = cursorPosition;

        }
    }
}
