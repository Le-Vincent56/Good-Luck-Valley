using GoodLuckValley.Player.Control;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Player.Input.MenuInputActions;

namespace GoodLuckValley.Player.Input
{
    [CreateAssetMenu(fileName = "MenuInputReader", menuName = "Input/Menu Input Reader")]
    public class MenuInputReader : ScriptableObject, IMenuControlsActions, IUIActions, IInputReader
    {
        public event UnityAction<bool> Start = delegate { };
        public event UnityAction<bool> Cancel = delegate { };
        public event UnityAction<bool> Click = delegate { };
        public event UnityAction<bool> MiddleClick = delegate { };
        public event UnityAction<Vector2> Navigate = delegate { };
        public event UnityAction<Vector2> Point = delegate { };
        public event UnityAction<bool> RightClick = delegate { };
        public event UnityAction<Vector2> ScrollWheel = delegate { };
        public event UnityAction<bool> Submit = delegate { };
        public event UnityAction<bool> Escape = delegate { };
        public event UnityAction<bool> AltModifier = delegate { };
        public event UnityAction<bool> ShiftModifier = delegate { };

        MenuInputActions inputActions;

        void OnEnable()
        {
            // Check if the input actions have been set
            if(inputActions == null)
            {
                // If not, create a new one and set callbacks
                inputActions = new MenuInputActions();
                inputActions.MenuControls.SetCallbacks(this);
                inputActions.UI.SetCallbacks(this);
            }

            // Enable the input actions
            Enable();

            // Enable the menu controls
            inputActions.MenuControls.Enable();
        }

        /// <summary>
        /// Enable the input actions map
        /// </summary>
        public void Enable() => inputActions.Enable();

        /// <summary>
        /// Disable the input actions map
        /// </summary>
        public void Disable() => inputActions.Disable();

        /// <summary>
        /// Switch to the UI Action Map
        /// </summary>
        public void SwitchToUIActionMap()
        {
            // Exit case - if the menu controls aren't enable
            if (!inputActions.MenuControls.enabled) return;

            // Disable menu controls and enable UI controls
            inputActions.MenuControls.Disable();
            inputActions.UI.Enable();
        }

        public void OnStart(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Start.Invoke(context.started);
            }
            else if(context.canceled)
            {
                Start.Invoke(context.started);
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Cancel.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Cancel.Invoke(context.started);
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Click.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Click.Invoke(context.started);
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                MiddleClick.Invoke(context.started);
            }
            else if (context.canceled)
            {
                MiddleClick.Invoke(context.started);
            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                Navigate.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Navigate.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                RightClick.Invoke(context.started);
            }
            else if (context.canceled)
            {
                RightClick.Invoke(context.started);
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Navigate.Invoke(context.ReadValue<Vector2>());
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Submit.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Submit.Invoke(context.started);
            }
        }

        public void OnEscape(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Escape.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Escape.Invoke(context.started);
            }
        }

        public void OnAltModifier(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AltModifier.Invoke(context.started);
            }
            else if (context.performed)
            {
                AltModifier.Invoke(context.performed);
            }
            else if (context.canceled)
            {
                AltModifier.Invoke(context.started);
            }
        }

        public void OnShiftModifier(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ShiftModifier.Invoke(context.started);
            }
            else if (context.performed)
            {
                ShiftModifier.Invoke(context.performed);
            }
            else if (context.canceled)
            {
                ShiftModifier.Invoke(context.started);
            }
        }

        public void OnOpenJournal(InputAction.CallbackContext context) { }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    }
}