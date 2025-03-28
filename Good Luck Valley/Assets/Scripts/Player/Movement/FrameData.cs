using GoodLuckValley.Input;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    public class FrameData
    {
        private PlayerController controller;
        private FrameInput frameInput;
        private bool hasInputThisFrame;
        private Vector2 trimmedFrameVelocity;
        private Vector2 framePosition;
        private Vector2 frameTransientVelocity;
        private Vector2 lastFrameTotalTransientVelocity;
        private Vector2 frameForceToApply;
        private float lastFrameY;

        public FrameInput Input { get => frameInput; }
        public bool HasInput { get => hasInputThisFrame; }
        public Vector2 TrimmedVelocity { get => trimmedFrameVelocity;  }
        public Vector2 Position { get => framePosition; }
        public Vector2 TransientVelocity 
        {
            get => frameTransientVelocity;
            set => frameTransientVelocity = value; 
        }

        public Vector2 PreviousTotalTransientVelocity 
        { 
            get => lastFrameTotalTransientVelocity; 
            set => lastFrameTotalTransientVelocity = value;  
        }

        public Vector2 ForceToApply { get => frameForceToApply; }

        public FrameData(PlayerController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Retrieve the input for the frame
        /// </summary>
        public void GatherInput(GameInputReader inputReader)
        {
            // Retrieve frame input
            frameInput = inputReader.RetrieveFrameInput();

            // Set jump inputs
            if(frameInput.JumpDown)
            {
                controller.Jump.JumpToConsume = true;
                controller.Jump.TimeJumpWasPressed = controller.Time;
            }
        }

        /// <summary>
        /// Set data for the frame
        /// </summary>
        public void Set()
        {
            float rotation = controller.RB.rotation * Mathf.Deg2Rad;

            // Set PlayerController direction vectors
            controller.Up = new Vector2(-Mathf.Sin(rotation), Mathf.Cos(rotation));
            controller.Right = new Vector2(controller.Up.y, -controller.Up.x);

            // Get the frame position
            framePosition = controller.RB.position;

            // Check if there's input during this frame
            hasInputThisFrame = frameInput.Move.x != 0;

            // Set the PlayerController's velocity and the trimmed velocity
            controller.Velocity = controller.RB.linearVelocity;
            trimmedFrameVelocity = new Vector2(controller.Velocity.x, 0);
        }

        /// <summary>
        /// Add a force to apply this frame
        /// </summary>
        public void AddForce(Vector2 force, bool resetVelocity = false, bool resetYOnly = false, bool resetXOnly = false)
        {
            // Check if resetting velocity
            if (resetVelocity)
            {
                // Check if resetting only the Y component
                if(resetYOnly)
                {
                    // Set the y-trimmed frame velocity
                    controller.SetVelocity(trimmedFrameVelocity);
                } else
                {
                    // Set the velocity to 0
                    controller.SetVelocity(Vector2.zero);
                }
            }

            // Add the force to the frame force to apply
            frameForceToApply += force;
        }

        /// <summary>
        /// Calculate any additional velocities for the frame
        /// </summary>
        public Vector2 AdditionalFrameVelocities()
        {
            if (controller.ImmediateMove.sqrMagnitude > controller.Collisions.SkinWidth)
            {
                controller.RB.MovePosition(framePosition + controller.ImmediateMove);
            }

            // Calculate the total transient velocity applied in the last frame and return
            lastFrameTotalTransientVelocity = frameTransientVelocity + controller.DecayingTransientVelocity;

            return lastFrameTotalTransientVelocity;
        }

        /// <summary>
        /// Handle end-of-frame cleaning
        /// </summary>
        public void Clean()
        {
            controller.Jump.JumpToConsume = false;
            frameForceToApply = Vector2.zero;
            lastFrameY = controller.Velocity.y;
        }
    }
}
