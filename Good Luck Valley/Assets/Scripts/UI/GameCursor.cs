using UnityEngine;
using UnityEngine.InputSystem;
using HiveMind.Core;
using HiveMind.Events;
using HiveMind.Menus;

namespace HiveMind.UI
{
    public class GameCursor : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] CameraScriptableObj cameraEvent;
        [SerializeField] DisableScriptableObj disableEvent;
        private GameObject player;
        private PauseMenu pauseMenu;
        #endregion

        #region FIELDS
        [SerializeField] private bool basedOnPlayer;
        private Vector2 cursorPosition;
        private Vector2 cursorVelocity = Vector3.zero;
        private Vector2 cursorDirection = Vector3.zero;
        private float cursorSpeed = 30f;
        private bool usingMouse = false;
        [SerializeField] private bool showDebugLines = false;
        #endregion

        private void OnEnable()
        {
            disableEvent.enableHUD.AddListener(EnableCursor);
            disableEvent.disableHUD.AddListener(DisableCursor);
        }

        private void OnDisable()
        {
            disableEvent.enableHUD.RemoveListener(EnableCursor);
            disableEvent.disableHUD.RemoveListener(DisableCursor);
        }

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;

            if (basedOnPlayer)
            {
                player = GameObject.Find("Player");
                pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
            }

            // Cursor fields
            cursorPosition = transform.position;

            // Confine the cursor to the window
            Cursor.lockState = CursorLockMode.Confined;
        }

        // Update is called once per frame
        void Update()
        {
            // Set cursor visibility to zero
            Cursor.visible = false;

            // Check if using mouse or gamepad input
            if (usingMouse)
            {
                // If using mouse, track mouse
                cursorDirection = Vector3.zero;
                cursorVelocity = Vector3.zero;
                cursorPosition = cameraEvent.GetMainCamera().ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y));
            }
            else
            {
                // Update cursor positiond with player
                if (basedOnPlayer)
                {
                    if (player.GetComponent<PlayerMovement>().IsMoving)
                    {
                        cursorPosition += player.GetComponent<PlayerMovement>().DistanceFromLastPosition;
                    }
                }

                // Move cursor with gamepad
                cursorVelocity = cursorDirection.normalized * cursorSpeed * Time.deltaTime;
                cursorPosition += cursorVelocity;
            }

            // Check bounds
            if (basedOnPlayer)
            {
                CheckCursorBoundsPlayer();
            }
            else
            {
                // If not based on player, the static camera will do
                CheckCursorBoundsStatic();
            }

            // Draw cursor
            transform.position = cursorPosition;
        }

        /// <summary>
        /// Check cursor bounds when there is a dynamic camera
        /// </summary>
        private void CheckCursorBoundsPlayer()
        {
            // Clamp the cursor position into the bounds of the screen
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, cameraEvent.GetLeftBound(), cameraEvent.GetRightBound());
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, cameraEvent.GetBottomBound(), cameraEvent.GetTopBound());
        }

        /// <summary>
        /// Check cursor bounds when there is a static camera
        /// </summary>
        private void CheckCursorBoundsStatic()
        {
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, -cameraEvent.GetCamWidth(), cameraEvent.GetCamWidth());
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, -cameraEvent.GetCamHeight(), cameraEvent.GetCamHeight());
        }

        private void EnableCursor()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }

        private void DisableCursor()
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        #region INPUT HANDLER
        /// <summary>
        /// OnAim event to trigger cursor movement
        /// </summary>
        /// <param name="context">The current controller context being used</param>
        public void OnAim(InputAction.CallbackContext context)
        {
            // Check if context is mouse or controller
            if (context.control.name == "position")
            {
                usingMouse = true;
            }
            else
            {
                // Controller read values
                usingMouse = false;
                cursorDirection = context.ReadValue<Vector2>();
            }
        }
        #endregion
    }
}
