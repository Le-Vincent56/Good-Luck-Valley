using GoodLuckValley.Input.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Input.Actions.GameplayActions;

namespace GoodLuckValley.Input
{
    [CreateAssetMenu(fileName = "UI Input Reader", menuName = "Input/UI Input Reader")]
    public class UIInputReader : ScriptableObject, IUIActions, IInputReader
    {
        public UnityAction<Vector2> Navigate = delegate { };
        public UnityAction<Vector2> Point = delegate { };
        public UnityAction<Vector2> Scrollwheel = delegate { };
        public UnityAction<bool> Submit = delegate { };
        public UnityAction<bool> Cancel = delegate { };
        public UnityAction<bool> Click = delegate { };
        public UnityAction<bool> RightClick = delegate { };
        public UnityAction<bool> MiddleClick = delegate { };
        public UnityAction<bool> AltModifier = delegate { };
        public UnityAction<bool> ShiftModifier = delegate { };
        public UnityAction<bool> Start = delegate { };
        

        private GameplayActions inputActions;

        private void OnEnable() => Enable();

        private void OnDisable() => Disable();

        /// <summary>
        /// Enable the input actions
        /// </summary>
        public void Enable()
        {
            // Check if the input actions have been initialized
            if (inputActions == null)
            {
                // Initialize the input actions and set callbacks
                inputActions = new GameplayActions();
                inputActions.UI.SetCallbacks(this);
            }

            // Enable the input actions
            inputActions.Enable();
        }

        /// <summary>
        /// Disable the input actions
        /// </summary>
        public void Disable() => inputActions.Disable();

        /// <summary>
        /// Set the UI Actions
        /// </summary>
        public void Set()
        {
            // Disable all other actions
            inputActions.Player.Disable();

            // Enable the UI actions
            inputActions.UI.Enable();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            Navigate.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Invoke the event and pass in the read value
            Point.Invoke(context.ReadValue<Vector2>());
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            // Invoke the event and pass in the read value
            Scrollwheel.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Submit.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Submit.Invoke(false);
                    break;
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Cancel.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Cancel.Invoke(false);
                    break;
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Click.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Click.Invoke(false);
                    break;
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    RightClick.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    RightClick.Invoke(false);
                    break;
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    MiddleClick.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    MiddleClick.Invoke(false);
                    break;
            }
        }

        public void OnAltModifier(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    AltModifier.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    AltModifier.Invoke(false);
                    break;
            }
        }

        public void OnShiftModifier(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    ShiftModifier.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    ShiftModifier.Invoke(false);
                    break;
            }
        }

        public void OnStart(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Start.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Start.Invoke(false);
                    break;
            }
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    }
}
