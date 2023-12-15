using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 RawMovementInput { get; private set; }
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }
        public bool JumpInput { get; private set; }
        float lastPressedJumpTime;


        public void OnMove(InputAction.CallbackContext context)
        {
            RawMovementInput = context.ReadValue<Vector2>();

            NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
            NormInputY = (int)(RawMovementInput * Vector2.right).normalized.y;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                JumpInput = true;
            }
        }

        public void UseJumpInput() => JumpInput = false;
    }
}
