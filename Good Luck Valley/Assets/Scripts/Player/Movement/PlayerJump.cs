using GoodLuckValley.Potentiates;
using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class PlayerJump
    {
        public enum JumpType
        {
            Jump,
            WallJump,
            Coyote,
            TimeJump,
            SlideJump
        }

        private PotentiateHandler potentiateHandler;
        private PlayerController controller;

        private const float JUMP_CLEARANCE_TIME = 0.25f;
        private float lastJumpExecutedTime;
        [SerializeField] private bool bufferedJumpUsable;
        [SerializeField] private bool jumpToConsume;
        private float timeJumpWasPressed;
        [SerializeField] private bool endedJumpEarly;
        [SerializeField] private bool hasTimeJump;
        [SerializeField] private bool coyoteUsable;
        private float timeLeftGrounded;

        public bool IsWithinJumpClearance { get => lastJumpExecutedTime + JUMP_CLEARANCE_TIME > controller.Time; }
        private bool HasBufferedJump
        {
            get => bufferedJumpUsable &&
                controller.Time < timeJumpWasPressed + controller.Stats.BufferedJumpTime &&
                !IsWithinJumpClearance;
        }

        private bool CanUseCoyote
        {
            get => coyoteUsable &&
                !controller.Collisions.Grounded &&
                controller.Time < timeLeftGrounded + controller.Stats.CoyoteTime;
        }

        public bool HasTimeJump { get => hasTimeJump; }
        private bool CanTimeJump { get => !controller.Collisions.Grounded && hasTimeJump; }
        public bool BufferedJumpUsable { get => bufferedJumpUsable; set => bufferedJumpUsable = value; }
        public bool JumpToConsume { get => jumpToConsume; set => jumpToConsume = value; }
        public float TimeJumpWasPressed { get => timeJumpWasPressed; set => timeJumpWasPressed = value; }
        public bool EndedJumpEarly { get => endedJumpEarly; }
        public bool CoyoteUsable { get => coyoteUsable; set => coyoteUsable = value; }
        public float TimeLeftGrounded { get => timeLeftGrounded; set => timeLeftGrounded = value; }
        public bool IsJumping { get; set; }

        public PlayerJump(PlayerController controller, PotentiateHandler potentiateHandler)
        {
            this.controller = controller;
            this.potentiateHandler = potentiateHandler;
        }

        /// <summary>
        /// Calculate the type of jump
        /// </summary>
        public void CalculateJump()
        {
            if ((jumpToConsume || HasBufferedJump) && controller.Crawl.CanStand)
            {      
                if (controller.WallJump.CanWallJump) ExecuteJump(JumpType.WallJump);
                else if (controller.Collisions.Grounded && !controller.Bounce.FromBounce) ExecuteJump(JumpType.Jump);
                else if (CanUseCoyote && !controller.Bounce.FromBounce) ExecuteJump(JumpType.Coyote);
                else if (CanTimeJump) ExecuteJump(JumpType.TimeJump);
            }

            // Check if the jump ended early
            if ((!endedJumpEarly && 
                    !controller.Collisions.Grounded 
                    && !controller.FrameData.Input.JumpHeld 
                    && controller.Velocity.y > 0
                ) 
                || controller.Velocity.y < 0
            )
            {
                endedJumpEarly = true;
            }

            // Check if the time has surpassed the time to return wall input loss
            if (controller.Time > controller.WallJump.ReturnWallInputLossAfter)
                // Change the wall jump input nerf point
                controller.WallJump.WallJumpInputNerfPoint = Mathf.MoveTowards(controller.WallJump.WallJumpInputNerfPoint, 1, controller.Delta / controller.Stats.WallJumpInputLossReturnTime);
        }

        /// <summary>
        /// Execute a Player jump
        /// </summary>
        private void ExecuteJump(JumpType jumpType)
        {
            // Set the PlayerController's velocity using the trimmed frame velocity
            controller.SetVelocity(controller.FrameData.TrimmedVelocity);

            // Set variables
            endedJumpEarly = false;
            bufferedJumpUsable = false;
            lastJumpExecutedTime = controller.Time;
            controller.Collisions.CurrentStepDownLength = 0;
            controller.ExtraConstantGravity = controller.Stats.JumpConstantGravity;

            // Check if jumping normally or using coyote time
            if (jumpType is JumpType.Jump or JumpType.Coyote)
            {
                // Disable coyote time
                coyoteUsable = false;

                // Add jump force
                controller.FrameData.AddForce(new Vector2(0, controller.Stats.JumpPower));
            }
            // Otherwise, check if jumping in mid-air
            else if (jumpType is JumpType.TimeJump)
            {
                // Remove the time jump
                hasTimeJump = false;

                // Add jump force
                controller.FrameData.AddForce(new Vector2(0, controller.Stats.JumpPower));

                // Deplete the last potentiate
                potentiateHandler.DepletePotentiate();
            }
            // Otherwise, check if performing a wall jump
            else if(jumpType is JumpType.WallJump)
            {
                // Execute the wall jump
                controller.WallJump.ExecuteWallJump();
            }

            //controller.Jumped?.Invoke(jumpType);
        }

        /// <summary>
        /// Add a Time Jump
        /// </summary>
        public void AddTimeJump() => hasTimeJump = true;

        /// <summary>
        /// Remove a Time Jump
        /// </summary>
        public void RemoveTimeJump() => hasTimeJump = false;
    }
}
