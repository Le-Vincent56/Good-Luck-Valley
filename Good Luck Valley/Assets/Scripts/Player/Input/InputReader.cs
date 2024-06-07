using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Player.Input.PlayerInputActions;

namespace GoodLuckValley.Player.Input
{
    public struct ContextData
    {
        public bool Started;
        public bool Canceled;

        public ContextData(bool started, bool canceled)
        {
            Started = started;
            Canceled = canceled;
        }
    }

    [CreateAssetMenu(fileName = "InputReader")]
    public class InputReader : ScriptableObject, IPlayerControlsActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> FastFall = delegate { };
        public event UnityAction<bool, bool> Throw = delegate { };
        public event UnityAction<bool> QuickBounce = delegate { };
        public event UnityAction<bool> RecallLast = delegate { };
        public event UnityAction<bool> RecallAll = delegate { };
        public event UnityAction<bool> Interact = delegate { };
        public event UnityAction<bool> Pause = delegate { };
        public event UnityAction<bool> DevTools = delegate { };
        public event UnityAction<bool> NoClip = delegate { };
        public event UnityAction<bool> UnlockPowers = delegate { };
        public event UnityAction<bool> Look = delegate { };

        PlayerInputActions inputActions;
        public Vector3 Direction => inputActions.PlayerControls.Move.ReadValue<Vector2>();
        public int NormMoveX { get; private set; }
        public int NormMoveY { get; private set; }
        public int NormLookY { get; private set; }

        void OnEnable()
        {
            // Check if the input actions have been set
            if(inputActions == null)
            {
                // If not, create a new one and set callbacks
                inputActions = new PlayerInputActions();
                inputActions.PlayerControls.SetCallbacks(this);
            }

            // Enable the input actions
            inputActions.Enable();
        }

        public void OnFastFall(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                FastFall.Invoke(context.started);
            }
            else if (context.canceled)
            {
                FastFall.Invoke(context.started);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            Interact.Invoke(context.started);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                Jump.Invoke(context.started);
            } else if(context.canceled)
            {
                Jump.Invoke(context.started);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 rawMovementInput = context.ReadValue<Vector2>();
            Move.Invoke(rawMovementInput);

            NormMoveX = (int)(rawMovementInput * Vector2.right).normalized.x;
            NormMoveY = (int)(rawMovementInput * Vector2.up).normalized.y;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            Pause.Invoke(context.started);
        }

        public void OnQuickBounce(InputAction.CallbackContext context)
        {
            QuickBounce.Invoke(context.started);
        }

        public void OnRecallAll(InputAction.CallbackContext context)
        {
            RecallAll.Invoke(context.started);
        }

        public void OnRecallLast(InputAction.CallbackContext context)
        {
            RecallLast.Invoke(context.started);
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                Throw.Invoke(context.started, context.canceled);
            } else if(context.canceled)
            {
                Throw.Invoke(context.started, context.canceled);
            }
        }

        public void OnDevTools(InputAction.CallbackContext context)
        {
            DevTools.Invoke(context.started);
        }

        public void OnNoClip(InputAction.CallbackContext context)
        {
            NoClip.Invoke(context.started);
        }

        public void OnUnlockPowers(InputAction.CallbackContext context)
        {
            UnlockPowers.Invoke(context.started);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 rawMovementInput = context.ReadValue<Vector2>();
            NormLookY = (int)(rawMovementInput * Vector2.up).normalized.y;

            if (context.started)
            {
                Look.Invoke(context.started);
            } else if(context.canceled)
            {
                Look.Invoke(context.started);
            }
        }
    }
}