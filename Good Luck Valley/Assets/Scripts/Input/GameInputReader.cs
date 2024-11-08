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
    }

    [CreateAssetMenu(fileName = "Game Input Reader", menuName = "Input/Game Input Reader")]
    public class GameInputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        public event UnityAction<Vector2, bool> Move = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        public int NormMoveX { get; private set; }
        public int NormMoveY { get; private set; }

        private GameplayActions inputActions;

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
        /// Retrieve input from this frame
        /// </summary>
        public FrameInput RetrieveFrameInput()
        {
            return new FrameInput
            {
                Move = inputActions.Player.Move.ReadValue<Vector2>(),
                JumpDown = inputActions.Player.Jump.WasPressedThisFrame(),
                JumpHeld = inputActions.Player.Jump.IsPressed()
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

                // If canceled, invoke with fals
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }
    }
}