using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GoodLuckValley.Level;
using UnityEngine.Rendering;

namespace GoodLuckValley.UI
{
    public class GameCursor : MonoBehaviour
    {
        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onGetCursorBounds;
        #endregion

        #region  FIELDS
        private Vector2 cursorPosition;
        private CameraData.ScreenBounds cursorBounds;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Set Unity cursor variables
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            cursorPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            // Set desired cursor position
            cursorPosition = Camera.main.ScreenToWorldPoint(
                new Vector2(
                    Mouse.current.position.ReadValue().x,
                    Mouse.current.position.ReadValue().y
                )
            );

            // Get cursor bounds
            // Calls to:
            //  - CameraData.OnGetCursorBounds();
            onGetCursorBounds.Raise(this, null);

            // Check cursor bounds based on the player
            CheckCursorBoundsPlayer();

            // Update actual position
            transform.position = cursorPosition;
        }

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

        private void CheckCursorBoundsPlayer()
        {
            // Clamp the cursor position into the bounds of the screen
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, cursorBounds.Left, cursorBounds.Right);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, cursorBounds.Bottom, cursorBounds.Top);
        }

        public void UpdateCursorBounds(Component sender, object data)
        {
            // Make sure the correct data was sent
            if(data is not CameraData.ScreenBounds) return;

            // Cast and update data
            cursorBounds = (CameraData.ScreenBounds)data;
        }
    }
}
