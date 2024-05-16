using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Player
{
    public class PlayerInputHandler : MonoBehaviour
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

        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onThrow;

        [Header("Objects")]
        [SerializeField] private PlayerData playerData;
        #endregion

        #region FIELDS
        public Vector2 RawMovementInput { get; private set; }
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }
        public bool JumpInput { get; private set; }
        public bool TryJumpCut { get; private set; }
        public float LastPressedJumpTime { get; set; }
        public bool FastFallInput { get; private set; }
        #endregion


        /// <summary>
        /// Handle Movement Input
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            // Retrieve the raw vector
            RawMovementInput = context.ReadValue<Vector2>();

            // Normalize inputs for consistent controller speeds
            NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
            NormInputY = (int)(RawMovementInput * Vector2.right).normalized.y;
        }

        /// <summary>
        /// Handle Jump Input
        /// </summary>
        /// <param name="context"></param>
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

        /// <summary>
        /// Handle Throw Input
        /// </summary>
        /// <param name="context"></param>
        public void OnThrow(InputAction.CallbackContext context)
        {
            // Get the contexts
            ContextData contextData = new ContextData(context.started, context.canceled);

            // Raise the throw event
            // Calls to:
            //  - MushroomThrow.OnThrow();
            onThrow.Raise(this, contextData);
        }

        public void OnFastFall(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                FastFallInput = true;
            } else if(context.canceled)
            {
                FastFallInput = false;
            }
        }

        /// <summary>
        /// Reset the Jump Input
        /// </summary>
        public void UseJumpInput() => JumpInput = false;
    }
}
