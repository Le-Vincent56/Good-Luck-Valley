using GoodLuckValley.Player.Data;
using GoodLuckValley.Player.Development;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class CollisionHandler
    {
        public enum ColliderMode
        {
            Standard,
            Crawling,
            Airborne
        }

        private PlayerController controller;
        private DevelopmentTools devTools;
        private BoxCollider2D boxCollider;
        private CapsuleCollider2D airborneCollider;

        private const float SKIN_WIDTH = 0.02f;
        private const int RAY_SIDE_COUNT = 5;
        private RaycastHit2D groundHit;
        private int lastGroundLayer;
        [SerializeField] private bool grounded;
        [SerializeField] private bool isOnSlope;
        [SerializeField] private bool isSliding;
        private float lastSteepSlopeAngle;
        private int slideDirection;
        private float currentStepDownLength;

        private float GrounderLength => controller.CharacterSize.StepHeight + SKIN_WIDTH;
        public bool IsOnSlope => isOnSlope;
        public bool IsSliding  { get => isSliding; set => isSliding = value; }
        public Vector2 RayPoint => controller.FrameData.Position + controller.Up * (controller.CharacterSize.StepHeight + SKIN_WIDTH);

        public bool Grounded { get => grounded; }
        public RaycastHit2D GroundHit { get => groundHit; }
        public Vector2 GroundNormal { get => groundHit.normal; }
        public float CurrentStepDownLength { get => currentStepDownLength; set => currentStepDownLength = value; }
        public float SkinWidth { get => SKIN_WIDTH; }
        public int LastGroundLayer { get => lastGroundLayer; }
        public int SlideDirection { get => slideDirection; }
        public float LastSteepSlopeAngle { get => lastSteepSlopeAngle; }

        public CollisionHandler(
            PlayerController controller,
            DevelopmentTools devTools,
            BoxCollider2D boxCollider,
            CapsuleCollider2D airborneCollider
        )
        {
            this.controller = controller;
            this.devTools = devTools;
            this.boxCollider = boxCollider;
            this.airborneCollider = airborneCollider;
        }

        /// <summary>
        /// Set up the Collision Handler
        /// </summary>
        public void Setup()
        {
            // Primary collider
            boxCollider = controller.GetComponent<BoxCollider2D>();
            boxCollider.edgeRadius = CharacterSize.COLLIDER_EDGE_RADIUS;
            boxCollider.hideFlags = HideFlags.NotEditable;
            boxCollider.sharedMaterial = controller.RB.sharedMaterial;
            boxCollider.enabled = true;

            // Airborne collider
            airborneCollider = controller.GetComponent<CapsuleCollider2D>();
            airborneCollider.hideFlags = HideFlags.NotEditable;
            airborneCollider.size = new Vector2(controller.CharacterSize.Width - SKIN_WIDTH * 2f, controller.CharacterSize.Height - SKIN_WIDTH * 2f);
            airborneCollider.offset = new Vector2(0, controller.CharacterSize.Height / 2f);
            airborneCollider.sharedMaterial = controller.RB.sharedMaterial;

            // Set the collider mode
            SetColliderMode(ColliderMode.Airborne);
        }

        /// <summary>
        /// Calculate collisions
        /// </summary>
        public void CalculateCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            float groundAngle = 0f;

            // Check if grounded in this frame
            bool isGroundedThisFrame = PerformRay(RayPoint, out groundAngle);

            // Check if not grounded
            if (!isGroundedThisFrame)
            {
                // Generate more rays and iterate through them
                foreach (float offset in GenerateRayOffsets())
                {
                    // Check if any of the rays hve detected ground
                    isGroundedThisFrame = PerformRay(RayPoint + controller.Right * offset, out groundAngle) 
                        || PerformRay(RayPoint - controller.Right * offset, out groundAngle);

                    // Exit case - if any rays have detected ground
                    if (isGroundedThisFrame) break;
                }
            }

            // Check for slope detection
            if (groundAngle != 0 && !isOnSlope) isOnSlope = true;
            else if (groundAngle == 0 && isOnSlope) isOnSlope = false;

            // Check if currently grounded, but not grounded before
            if (isGroundedThisFrame && !grounded) 
            {
                // Check if the player is on slope (this accounts for upwards movement)
                if(isOnSlope)
                {
                    ToggleGrounded(true);
                } else if(controller.RB.linearVelocity.y <= 0)
                {
                    // Set grounded
                    ToggleGrounded(true);
                }
            } 
            // Check if currently not grounded, but was grounded
            else if (!isGroundedThisFrame && grounded) 
                // Set not grounded
                ToggleGrounded(false);

            // Reset the Physics2D query
            Physics2D.queriesStartInColliders = controller.CachedQueryMode;
        }

        /// <summary>
        /// Cast a ray to detect the ground
        /// </summary>
        private bool PerformRay(Vector2 point, out float groundAngle)
        {
            // Cast a ray to detect the ground
            groundHit = Physics2D.Raycast(point, -controller.Up, GrounderLength + currentStepDownLength, controller.Stats.CollisionLayers);

            // Get the angle of the ground
            groundAngle = Vector2.Angle(groundHit.normal, controller.Up);

            // Debug
            Debug.DrawRay(point, -controller.Up * (GrounderLength + currentStepDownLength), Color.blue);

            // If no ground detected, return false
            if (!groundHit)
            {
                isSliding = false;
                return false;
            }

            // Exit case - if the angle from the ground hit normal and the up vector is greater than the max walkable slope
            if (groundAngle > controller.Stats.MaxWalkableSlope)
            {
                // Start sliding and remove player movement
                isOnSlope = true;
                isSliding = true;
                controller.Active = false;
                slideDirection = (int)Mathf.Sign(groundHit.normal.x);
                lastSteepSlopeAngle = groundAngle;

                return false;
            }

            // Stop sliding and allow for player movement
            isSliding = false;
            controller.Active = true;

            // Get the layer of the ground hit object
            lastGroundLayer = groundHit.collider.gameObject.layer;

            // If ground detected, return true
            return true;
        }

        /// <summary>
        /// Generate an enumerable set of rays based on offsets
        /// </summary>
        public IEnumerable<float> GenerateRayOffsets()
        {
            // Get the extent of the rays
            float extent = controller.CharacterSize.StandingColliderSize.x / 2 - controller.CharacterSize.RayInset;
            
            // Get the offset amount
            float offsetAmount = extent / RAY_SIDE_COUNT;
            
            // Iterate through the amount of rays
            for (int i = 1; i < RAY_SIDE_COUNT + 1; i++)
            {
                // Multiple the ray by the offset
                yield return offsetAmount * i;
            }
        }

        /// <summary>
        /// Toggle whether or not the player is grounded
        /// </summary>
        public void ToggleGrounded(bool grounded)
        {
            this.grounded = grounded;
            if (grounded)
            {
                //GroundedChanged?.Invoke(true, _lastFrameY);
                controller.RB.gravityScale = 0;
                controller.SetVelocity(controller.FrameData.TrimmedVelocity);
                controller.ConstantForce.force = Vector2.zero;
                currentStepDownLength = controller.CharacterSize.StepHeight;
                controller.Jump.CoyoteUsable = true;
                controller.Jump.BufferedJumpUsable = true;
                controller.WallJump.IsWallJumping = false;
                controller.WallJump.FromWallJump = false;
                controller.Bounce.ResetBounce();
                SetColliderMode(ColliderMode.Standard);
            }
            else
            {
                //GroundedChanged?.Invoke(false, 0);
                controller.Jump.TimeLeftGrounded = controller.Time;

                // Check if not noclipping
                if(!devTools.NoClip)
                    // Set the gravity scale
                    controller.RB.gravityScale = controller.Stats.JumpGravityScale;

                // Set slope variables
                isOnSlope = false;
                isSliding = false;

                SetColliderMode(ColliderMode.Airborne);
            }
        }

        /// <summary>
        /// Set the collider mode for the Player
        /// </summary>
        public void SetColliderMode(ColliderMode mode)
        {
            airborneCollider.enabled = mode == ColliderMode.Airborne;

            switch (mode)
            {
                case ColliderMode.Standard:
                    boxCollider.size = controller.CharacterSize.StandingColliderSize;
                    boxCollider.offset = controller.CharacterSize.StandingColliderCenter;
                    break;
                case ColliderMode.Crawling:
                    boxCollider.size = controller.CharacterSize.CrouchColliderSize;
                    boxCollider.offset = controller.CharacterSize.CrouchingColliderCenter;
                    break;
                case ColliderMode.Airborne:
                    break;
            }
        }
    }
}