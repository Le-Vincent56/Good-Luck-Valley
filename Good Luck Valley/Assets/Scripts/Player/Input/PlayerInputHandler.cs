using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        public Vector2 RawMovementInput { get; private set; }
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }
        public bool JumpInput { get; private set; }
        public bool TryJumpCut { get; private set; }
        public float LastPressedJumpTime { get; set; }


        public void OnMove(InputAction.CallbackContext context)
        {
            // Retrieve the raw vector
            RawMovementInput = context.ReadValue<Vector2>();

            // Normalize inputs for consistent controller speeds
            NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
            NormInputY = (int)(RawMovementInput * Vector2.right).normalized.y;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Check if the jump button has been pressed
            if(context.started)
            {
                // Set jump variables
                JumpInput = true;
                TryJumpCut = false;
                LastPressedJumpTime = playerData.jumpInputBufferTime;
            } else if(context.canceled) // Check if the jump button has been released
            {
                // Attempt to jump cut
                TryJumpCut = true;
            }
        }

        public void UseJumpInput() => JumpInput = false;
    }
}
