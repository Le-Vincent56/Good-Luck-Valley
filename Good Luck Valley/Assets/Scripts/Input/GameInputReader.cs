using GoodLuckValley.Input.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Input.Actions.GameplayActions;

namespace GoodLuckValley.Input
{
    public struct FrameInput
    {
        public Vector2 Move;
        public bool JumpDown;
        public bool JumpHeld;
        public bool Crawling;
        public bool Sliding;
        public bool SlowFalling;
    }

    [CreateAssetMenu(fileName = "Game Input Reader", menuName = "Input/Game Input Reader")]
    public class GameInputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        public event UnityAction<Vector2, bool> Move = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Crawl = delegate { };
        public event UnityAction<bool> Slide = delegate { };
        public event UnityAction<bool> Bounce = delegate { };
        public event UnityAction<bool> SlowFall = delegate { };
        public event UnityAction<bool> Interact = delegate { };
        public event UnityAction<bool> Recall = delegate { };
        public event UnityAction<bool> Journal = delegate { };
        public event UnityAction<bool> Pause = delegate { };

        public event UnityAction<bool> Dev = delegate { };
        public event UnityAction<bool> NoClip = delegate { };

        public int NormMoveX { get; private set; }
        public int NormMoveY { get; private set; }

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
                inputActions.Player.SetCallbacks(this);
            }

            // Enable the input actions
            inputActions.Enable();
        }

        /// <summary>
        /// Disable the input actions
        /// </summary>
        public void Disable() => inputActions.Disable();

        /// <summary>
        /// Set the Player Actions
        /// </summary>
        public void Set()
        {
            // Disable all other actions
            inputActions.UI.Disable();

            // Enable the Player actions
            inputActions.Player.Enable();
        }

        /// <summary>
        /// Load the input bindings from a JSON string
        /// </summary>
        public void LoadBindings(string bindingsJSON) => inputActions.LoadBindingOverridesFromJson(bindingsJSON);

        /// <summary>
        /// Retrieve input from this frame
        /// </summary>
        public FrameInput RetrieveFrameInput()
        {
            return new FrameInput
            {
                Move = inputActions.Player.Move.ReadValue<Vector2>(),
                JumpDown = inputActions.Player.Jump.WasPressedThisFrame(),
                JumpHeld = inputActions.Player.Jump.IsPressed(),
                Crawling = inputActions.Player.Crawl.IsPressed(),
                SlowFalling = inputActions.Player.SlowFall.IsPressed()
            };
        }

        /// <summary>
        /// Callback function to handle Movement input
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            // Get the raw movement input from the control
            Vector2 rawMovementInput = context.ReadValue<Vector2>();

            // Invoke the movement event
            Move.Invoke(rawMovementInput, context.started);

            // Set variables
            NormMoveX = (int)(rawMovementInput * Vector2.right).normalized.x;
            NormMoveY = (int)(rawMovementInput * Vector2.up).normalized.y;
        }

        /// <summary>
        /// Callback function to handle Jump input
        /// </summary>
        public void OnJump(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback function to handle Crawl input
        /// </summary>
        public void OnCrawl(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Crawl.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Crawl.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback function to handle Bounce input
        /// </summary>
        public void OnBounce(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Bounce.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Bounce.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback function to handle Slow Fall input
        /// </summary>
        public void OnSlowFall(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    SlowFall.Invoke(true);
                    break;

                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    SlowFall.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback function to handle Interact input
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Interact.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Interact.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// Callback function to handle Recall input
        /// </summary>
        public void OnRecall(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Recall.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Recall.Invoke(false);
                    break;
            }
        }

        public void OnDev(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Dev.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Dev.Invoke(false);
                    break;
            }
        }

        public void OnNoClip(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    NoClip.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    NoClip.Invoke(false);
                    break;
            }
        }

        public void OnJournal(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Journal.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Journal.Invoke(false);
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            // Check the context phase
            switch (context.phase)
            {
                // If starting, invoke with true
                case InputActionPhase.Started:
                    Pause.Invoke(true);
                    break;
                // If canceled, invoke with false
                case InputActionPhase.Canceled:
                    Pause.Invoke(false);
                    break;
            }
        }
    }
}