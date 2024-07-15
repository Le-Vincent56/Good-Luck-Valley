using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Player.Input.JournalInputActions;

namespace GoodLuckValley.Player.Input
{
    [CreateAssetMenu(fileName = "JournalInputReader", menuName = "Input/Journal Input Reader")]
    public class JournalInputReader : ScriptableObject, IJournalControlsActions
    {
        JournalInputActions inputActions;

        public event UnityAction<bool> Back = delegate { };
        public event UnityAction<bool> Read = delegate { };
        public event UnityAction<bool> NextPage = delegate { };
        public event UnityAction<bool> PrevPage = delegate { };

        private void OnEnable()
        {
            // Check if the input actions have been set
            if (inputActions == null)
            {
                // If not, create a new one and set callbacks
                inputActions = new JournalInputActions();
                inputActions.JournalControls.SetCallbacks(this);
            }

            // Enable the input actions
            inputActions.Enable();
        }

        /// <summary>
        /// Enable the input actions map
        /// </summary>
        public void Enable() => inputActions.Enable();

        /// <summary>
        /// Disable the input actions map
        /// </summary>
        public void Disable() => inputActions.Disable();

        public void OnBack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Back.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Back.Invoke(context.started);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Read.Invoke(context.started);
            }
            else if (context.canceled)
            {
                Read.Invoke(context.started);
            }
        }

        public void OnNextPage(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                NextPage.Invoke(context.started);
            }
            else if (context.canceled)
            {
                NextPage.Invoke(context.started);
            }
        }

        public void OnPreviousPage(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                PrevPage.Invoke(context.started);
            }
            else if (context.canceled)
            {
                PrevPage.Invoke(context.started);
            }
        }
    }
}