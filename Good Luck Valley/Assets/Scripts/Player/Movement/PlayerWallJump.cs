using GoodLuckValley.Events;
using GoodLuckValley.Events.Animation;
using GoodLuckValley.Player.Data;
using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class PlayerWallJump
    {
        private const float WALL_REATTACH_COOLDOWN = 0.2f;

        private PlayerController controller;
        private Bounds wallDetectionBounds;
        private RaycastHit2D wallHit;
        [SerializeField] private bool isOnWall;
        [SerializeField] private bool wallJumpCoyoteUsable;
        [SerializeField] private int wallDirection;
        [SerializeField] private int wallDirectionThisFrame;
        [SerializeField] private float canGrabWallAfter;
        [SerializeField] private float timeLeftWall;
        [SerializeField] private bool isWallJumping;
        [SerializeField] private bool fromWallJump;
        [SerializeField] private bool stickToWall;
        private int lastWallLayer;
        private int wallDirectionForJump;
        private float returnWallInputLossAfter;
        private float wallJumpInputNerfPoint;
        private event Action<bool> OnWallGrabChanged = delegate { };
        public event Action<int> OnWallJump = delegate { };
        public Bounds WallDetectionBounds { get => wallDetectionBounds; }
        public bool IsOnWall { get => isOnWall; }
        public bool CanWallJump 
        { 
            get
            {
                return !controller.Collisions.Grounded && 
                    (isOnWall || wallDirectionThisFrame != 0) || 
                    (wallJumpCoyoteUsable && controller.Time < timeLeftWall + controller.Stats.WallCoyoteTime);
            } 
        }
        private bool HorizontalInputPressed => Mathf.Abs(controller.FrameData.Input.Move.x) > controller.Stats.HorizontalDeadZoneThreshold;
        private bool IsPushingAgainstWall => HorizontalInputPressed && (int)Mathf.Sign(controller.Direction.x) == wallDirectionThisFrame;
        public int WallDirectionForJump { get => wallDirectionForJump; set => wallDirectionForJump = value; }
        public float ReturnWallInputLossAfter { get => returnWallInputLossAfter; set => returnWallInputLossAfter = value; }
        public float WallJumpInputNerfPoint { get => wallJumpInputNerfPoint; set => wallJumpInputNerfPoint = value; }
        public bool FromWallJump { get => fromWallJump; set => fromWallJump = value; }
        public bool IsWallJumping { get => isWallJumping; set => isWallJumping = value; }
        public int WallDirectionThisFrame { get => wallDirectionThisFrame; }
        public int LastWallLayer { get => lastWallLayer; }
        public RaycastHit2D WallHit { get => wallHit; }
        public bool StickToWall { get => stickToWall; }

        public PlayerWallJump(PlayerController controller)
        {
            this.controller = controller;

            // Set wall detection bounds
            wallDetectionBounds = new Bounds(
                new Vector3(0, controller.CharacterSize.Height / 2),
                new Vector3(controller.CharacterSize.StandingColliderSize.x + CharacterSize.COLLIDER_EDGE_RADIUS * 2 + controller.Stats.WallDetectorRange, 
                    controller.CharacterSize.Height - 0.1f)
            );
        }

        /// <summary>
        /// Toggle whether or not the Player is on the wall
        /// </summary>
        public void ToggleOnWall(bool on)
        {
            // Set whether or not the Player is on the wall
            isOnWall = on;

            // Check if on the wall
            if (on)
            {
                // Set variables
                controller.DecayingTransientVelocity = Vector2.zero;
                controller.Jump.BufferedJumpUsable = true;
                wallJumpCoyoteUsable = true;
                wallDirection = wallDirectionThisFrame;
                fromWallJump = false;
                controller.Bounce.ResetBounce();
                controller.Collisions.SetColliderMode(CollisionHandler.ColliderMode.Standard);
                stickToWall = true;
            }
            // Otherwise
            else
            {
                // Set variables
                timeLeftWall = controller.Time;
                canGrabWallAfter = controller.Time + WALL_REATTACH_COOLDOWN;
                wallDirection = 0;
                stickToWall = false;
            }

            // Call the event
            OnWallGrabChanged?.Invoke(on);
        }

        /// <summary>
        /// Calculate the current wall state
        /// </summary>
        public void CalculateWalls()
        {
            // Exit case -  do not allow wall jumping
            if (!controller.Stats.AllowWalls) return;

            // Exit case - if forcing a move
            if (controller.ForcedMove) return;

            // Get the raycast direction
            float rayDir = isOnWall ? wallDirection : wallDirectionThisFrame;

            // Cast to a potential wall
            bool hasHitWall = DetectWallCast(rayDir);

            // Get the wall direction this frame
            wallDirectionThisFrame = hasHitWall ? (int)rayDir : 0;

            // Check if not already on the wall, if the Player should stick to the wall, if the time is greater than the buffer, and the player is falling
            if (!isOnWall && ShouldStickToWall() && controller.Time > canGrabWallAfter && controller.Velocity.y < 0) 
                // Set to on the wall
                ToggleOnWall(true);
            // Otherwise, check if already on the wall and the Player should not longer stick to the wall
            else if (isOnWall && !ShouldStickToWall()) 
                // Stop being on the wall
                ToggleOnWall(false);

            // Check if not grabbing the wall and not grounded
            if (!isOnWall)
            {
                // Check if against a wall
                if (DetectWallCast(-1)) wallDirectionThisFrame = -1;
                else if (DetectWallCast(1)) wallDirectionThisFrame = 1;
            }
        }

        /// <summary>
        /// Check if the Player should stick to the wall
        /// </summary>
        private bool ShouldStickToWall()
        {
            if (wallDirectionThisFrame == 0 || controller.Collisions.Grounded) return false;

            if (HorizontalInputPressed && !IsPushingAgainstWall) return false; // If pushing away
            return !controller.Stats.RequireInputPush || (IsPushingAgainstWall);
        }

        /// <summary>
        /// Detect a wall using a Boxcast
        /// </summary>
        private bool DetectWallCast(float dir)
        {
            // Check for walls using a BoxCast
            wallHit = Physics2D.BoxCast(
                controller.FrameData.Position + (Vector2)wallDetectionBounds.center,
                new Vector2(
                    controller.CharacterSize.StandingColliderSize.x - controller.Collisions.SkinWidth,
                    wallDetectionBounds.size.y
                ),
                0,
                new Vector2(dir, 0),
                controller.Stats.WallDetectorRange,
                controller.Stats.ClimbableLayer
            );

            // Check if the BoxCast hit a wall
            if (wallHit)
                // Set the last wall layer
                lastWallLayer = wallHit.collider.gameObject.layer;

            return wallHit;
        }

        /// <summary>
        /// Exefcute a Wall Jump
        /// </summary>
        public void ExecuteWallJump()
        {
            // Toggle off the wall
            ToggleOnWall(false);

            // Set variables
            wallJumpCoyoteUsable = false;
            wallJumpInputNerfPoint = 0;
            ReturnWallInputLossAfter = controller.Time + controller.Stats.WallJumpTotalInputLossTime;
            wallDirectionForJump = wallDirectionThisFrame;
            isWallJumping = true;
            fromWallJump = true;

            controller.FrameData.AddForce(new Vector2(-wallDirectionThisFrame, 1) * controller.Stats.WallJumpPower, true);

            // Invoke the wall jump
            EventBus<ForceDirectionChange>.Raise(new ForceDirectionChange { DirectionToFace = wallDirectionThisFrame * -1, BufferUpdate = true });
        }
    }
}
