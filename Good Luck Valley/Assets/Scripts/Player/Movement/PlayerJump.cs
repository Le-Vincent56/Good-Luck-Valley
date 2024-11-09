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
            AirJump,
            SlideJump
        }

        private PlayerController controller;

        private const float JUMP_CLEARANCE_TIME = 0.25f;
        private float lastJumpExecutedTime;
        [SerializeField] private bool bufferedJumpUsable;
        [SerializeField] private bool jumpToConsume;
        private float timeJumpWasPressed;
        [SerializeField] private bool endedJumpEarly;
        [SerializeField] private int airJumpsRemaining;
        private bool wallJumpCoyoteUsable;
        [SerializeField] private bool coyoteUsable;
        private float timeLeftGrounded;
        private float returnWallInputLossAfter;

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

        private bool CanAirJump { get => !controller.Collisions.Grounded && airJumpsRemaining > 0; }
        //private bool CanWallJump
        //{
        //    get => controller.Collisions.Grounded &&
        //        isOnWall
        //}
        public bool BufferedJumpUsable { get => bufferedJumpUsable; set => bufferedJumpUsable = value; }
        public bool JumpToConsume { get => jumpToConsume; set => jumpToConsume = value; }
        public float TimeJumpWasPressed { get => timeJumpWasPressed; set => timeJumpWasPressed = value; }
        public bool EndedJumpEarly { get => endedJumpEarly; }
        public bool CoyoteUsable { get => coyoteUsable; set => coyoteUsable = value; }
        public float TimeLeftGrounded { get => timeLeftGrounded; set => timeLeftGrounded = value; }

        public PlayerJump(PlayerController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Calculate the type of jump
        /// </summary>
        public void CalculateJump()
        {
            if((jumpToConsume || HasBufferedJump) && controller.Crawl.CanStand)
            {
                //if (CanWallJump) ExecuteJump(JumpType.WallJump);
                if (controller.Collisions.Grounded)
                {
                    if (controller.Slide.Sliding) ExecuteJump(JumpType.SlideJump);
                    else ExecuteJump(JumpType.Jump);
                }
                else if (CanUseCoyote) ExecuteJump(JumpType.Coyote);
                else if (CanAirJump) ExecuteJump(JumpType.AirJump);
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

            // Check if jumping normally or using coyote time
            if(jumpType is JumpType.Jump or JumpType.Coyote or JumpType.SlideJump)
            {
                // Disable coyote time
                coyoteUsable = false;

                // Add jump force
                controller.FrameData.AddForce(new Vector2(0, controller.Stats.JumpPower));

                // Check to notify slide jumping
                if (jumpType is JumpType.SlideJump)
                {
                    controller.Slide.SlideJumping = true;
                }
                else controller.Slide.SlideJumping = false;
            }
            // Otherwise, check if jumping in mid-air
            else if (jumpType is JumpType.AirJump)
            {
                // Reduce the amount of air jumps remaining
                airJumpsRemaining--;

                // Add jump force
                controller.FrameData.AddForce(new Vector2(0, controller.Stats.JumpPower));
            }
            // Otherwise, check if performing a wall jump
            else if(jumpType is JumpType.WallJump)
            {
                // TODO: Wall stuff
            }

            //controller.Jumped?.Invoke(jumpType);
        }

        /// <summary>
        /// Reset the amount of air jumps the Player has
        /// </summary>
        public void ResetAirJumps() => airJumpsRemaining = controller.Stats.MaxAirJumps;
    }
}
