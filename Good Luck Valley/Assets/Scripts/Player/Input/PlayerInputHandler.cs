using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Player.Handlers
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

        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onThrow;
        [SerializeField] private GameEvent onRecallLast;
        [SerializeField] private GameEvent onRecallAll;
        [SerializeField] private GameEvent onRequestWallData;
        [SerializeField] private GameEvent onWallJumpInput;
        [SerializeField] private GameEvent onQuickBounce;
        [SerializeField] private GameEvent onInteract;
        [SerializeField] private GameEvent onPause;
        #endregion

        #region REFERENCES
        [Header("Objects")]
        [SerializeField] private PlayerDataOld playerData;
        #endregion

        #region FIELDS
        private bool paused;
        #endregion

        #region PROPERTIES
        public Vector2 RawMovementInput { get; private set; }
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }
        public bool JumpInput { get; private set; }
        public bool TryJumpCut { get; private set; }
        public float LastPressedJumpTime { get; set; }
        public bool FastFallInput { get; private set; }
        public bool OnWall { get; set; }
        public float WallDirection { get; set; }
        public Vector2 WallCheckPos { get; set; }
        #endregion


        /// <summary>
        /// Handle Movement input
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
        /// Handle Jump input
        /// </summary>
        /// <param name="context"></param>
        public void OnJump(InputAction.CallbackContext context)
        {
            // Request wall data
            onRequestWallData.Raise(this, null);

            // Check if the jump button has been pressed
            if(context.started)
            {
                // Check if on a wall
                if(!OnWall)
                {
                    // Set jump variables
                    JumpInput = true;
                    TryJumpCut = false;
                    LastPressedJumpTime = playerData.jumpInputBufferTime;
                } else
                {
                    // Send wall data if on a wall
                    MushroomWallJump.Data data = new MushroomWallJump.Data(true, WallCheckPos, WallDirection, playerData.mushroomWallRadius);
                    onWallJumpInput.Raise(this, data);
                }
            } else if(context.canceled) // Check if the jump button has been released
            {
                // Check if on a wall
                if(!OnWall)
                {
                    // Attempt to jump cut
                    TryJumpCut = true;
                }

                // Always cancel wall data send
                MushroomWallJump.Data data = new MushroomWallJump.Data(false, WallCheckPos, 0f, playerData.mushroomWallRadius);
                onWallJumpInput.Raise(this, data);
            }
        }

        /// <summary>
        /// Handle Throw input
        /// </summary>
        /// <param name="context"></param>
        public void OnThrow(InputAction.CallbackContext context)
        {
            // Return if paused
            if (paused) return;

            // Get the contexts
            ContextData contextData = new ContextData(context.started, context.canceled);

            // Raise the throw event
            // Calls to:
            //  - MushroomThrow.OnThrow();
            onThrow.Raise(this, contextData);
        }

        /// <summary>
        /// Handle Fast Fall input
        /// </summary>
        /// <param name="context"></param>
        public void OnFastFall(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                FastFallInput = true;
            } else if(context.canceled)
            {
                FastFallInput = false;
            }
        }

        /// <summary>
        /// Handle Recall Last input
        /// </summary>
        /// <param name="context"></param>
        public void OnRecallLast(InputAction.CallbackContext context)
        {
            // Return if paused
            if (paused) return;

            // Ensure a single button press
            if (context.started)
            {
                // Raise the recall all event
                // Calls to:
                //  - MushroomTracker.RecallLast();
                onRecallLast.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Recall All input
        /// </summary>
        /// <param name="context"></param>
        public void OnRecallAll(InputAction.CallbackContext context)
        {
            // Return if paused
            if (paused) return;

            // Ensure a single button press
            if (context.started)
            {
                // Raise the recall all event
                // Calls to:
                //  - MushroomTracker.RecallAll();
                onRecallAll.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Quick Bounce input
        /// </summary>
        /// <param name="context"></param>
        public void OnQuickBounce(InputAction.CallbackContext context)
        {
            // Return if paused
            if (paused) return;

            // Ensure a single button press
            if (context.started)
            {
                onQuickBounce.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Interact input
        /// </summary>
        /// <param name="context"></param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // Return if paused
            if (paused) return;

            // Ensure a single button press
            if (context.started)
            {
                onInteract.Raise(this, null);
            }
        }

        /// <summary>
        /// Handle Pause input
        /// </summary>
        /// <param name="context"></param>
        public void OnPause(InputAction.CallbackContext context)
        {
            // Ensure a single button press
            if(context.started)
            {
                onPause.Raise(this, null);
            }
        }

        /// <summary>
        /// Reset the Jump Input
        /// </summary>
        public void UseJumpInput() => JumpInput = false;

        public void SetPaused(Component sender, object data)
        {
            // Check that the data is the correct type
            if (data is not bool) return;

            // Set paused
            bool paused = (bool)data;
            this.paused = paused;
        }
    }
}
