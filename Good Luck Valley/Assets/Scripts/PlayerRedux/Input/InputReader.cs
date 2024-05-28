using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GoodLuckValley.Player.Input.PlayerInputActions;

namespace GoodLuckValley.Player.Input
{
    [CreateAssetMenu(fileName = "InputReader")]
    public class InputReader : ScriptableObject, IPlayerControlsActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool, bool> Throw = delegate { };
        public event UnityAction<bool> FastFall = delegate { };

        public void OnFastFall(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnQuickBounce(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnRecallAll(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnRecallLast(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}