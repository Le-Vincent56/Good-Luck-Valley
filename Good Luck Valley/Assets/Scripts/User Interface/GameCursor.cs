using UnityEngine;
using UnityEngine.InputSystem;
using GoodLuckValley.Cameras;
using GoodLuckValley.Events;

namespace GoodLuckValley.UI
{
    public class GameCursor : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onUpdateCursorPosition;

        [Header("Fields")]
        [SerializeField] private Vector2 cursorPosition;
        [SerializeField] private Bounds cursorBounds;

        // Start is called before the first frame update
        void Start()
        {
            // Set Unity cursor variables
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            cursorPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Reset cursor visibility if it's ever visible
            if(Cursor.visible)
                Cursor.visible = false;

            // Set desired cursor position
            cursorPosition = Camera.main.ScreenToWorldPoint(
                new Vector2(
                    Mouse.current.position.ReadValue().x,
                    Mouse.current.position.ReadValue().y
                )
            );

            // Check cursor bounds based on the player
            CheckCursorBoundsPlayer();

            // Update actual position
            transform.position = cursorPosition;

            // Send out cursor position
            // Calls to:
            //  - CameraOmniPeek.UpdateCursorPosition()
            onUpdateCursorPosition.Raise(this, cursorPosition);
        }

        /// <summary>
        /// Enable/Disable cursor visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnHandleCursorVisibility(Component sender, object data)
        {
            // Check if the correct data type
            if (data is not bool) return;

            // Cast data
            bool show = (bool)data;

            // Choose whether the show or hide the cursor
            if (show)
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            else
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        /// <summary>
        /// Clamp the cursor position within the screen bounds
        /// </summary>
        private void CheckCursorBoundsPlayer()
        {
            // Clamp the cursor position into the bounds of the screen
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, cursorBounds.min.x, cursorBounds.max.x);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, cursorBounds.min.y, cursorBounds.max.y);
        }

        /// <summary>
        /// Update the bounds that the cursor is restricted to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void UpdateCursorBounds(Component sender, object data)
        {
            // Make sure the correct data was sent
            if(data is not Bounds) return;

            // Cast and update data
            cursorBounds = (Bounds)data;
        }

        /// <summary>
        /// Incorporate cursor position into the throw position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SetThrowData(Component sender, object data)
        {
            if (sender is not ThrowLine) return;

            ((ThrowLine)sender).SetCursorPosition((Vector2)transform.position);
        }
    }
}
