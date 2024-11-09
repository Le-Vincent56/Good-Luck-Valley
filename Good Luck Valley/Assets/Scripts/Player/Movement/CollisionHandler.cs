using GoodLuckValley.Player.Data;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    public class CollisionHandler
    {
        public enum ColliderMode
        {
            Standard,
            Crawling,
            Airborne
        }

        private PlayerController controller;
        private BoxCollider2D boxCollider;
        private CapsuleCollider2D airborneCollider;

        private const float SKIN_WIDTH = 0.02f;
        private const int RAY_SIDE_COUNT = 5;
        private RaycastHit2D groundHit;
        private bool grounded;
        private float currentStepDownLength;

        private float GrounderLength => controller.CharacterSize.StepHeight + SKIN_WIDTH;

        public Vector2 RayPoint => controller.FrameData.Position + controller.Up * (controller.CharacterSize.StepHeight + SKIN_WIDTH);

        public bool Grounded { get => grounded; }
        public RaycastHit2D GroundHit { get => groundHit; }
        public Vector2 GroundNormal { get => groundHit.normal; }
        public float CurrentStepDownLength { get => currentStepDownLength; set => currentStepDownLength = value; }
        public float SkinWidth { get => SKIN_WIDTH; }

        public CollisionHandler(
            PlayerController controller,
            BoxCollider2D boxCollider,
            CapsuleCollider2D airborneCollider
        )
        {
            this.controller = controller;
            this.boxCollider = boxCollider;
            this.airborneCollider = airborneCollider;
        }

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
            airborneCollider.size = new Vector2(controller.CharacterSize.Width - SKIN_WIDTH * 2, controller.CharacterSize.Height - SKIN_WIDTH * 2);
            airborneCollider.offset = new Vector2(0, controller.CharacterSize.Height / 2);
            airborneCollider.sharedMaterial = controller.RB.sharedMaterial;

            // Set the collider mode
            SetColliderMode(ColliderMode.Airborne);
        }

        public void CalculateCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            bool isGroundedThisFrame = PerformRay(RayPoint);

            if (!isGroundedThisFrame)
            {
                foreach (float offset in GenerateRayOffsets())
                {
                    isGroundedThisFrame = PerformRay(RayPoint + controller.Right * offset) || PerformRay(RayPoint - controller.Right * offset);
                    if (isGroundedThisFrame) break;
                }
            }

            if (isGroundedThisFrame && !grounded) ToggleGrounded(true);
            else if (!isGroundedThisFrame && grounded) ToggleGrounded(false);

            Physics2D.queriesStartInColliders = controller.CachedQueryMode;

            bool PerformRay(Vector2 point)
            {
                groundHit = Physics2D.Raycast(point, -controller.Up, GrounderLength + currentStepDownLength, controller.Stats.CollisionLayers);
                if (!groundHit) return false;

                return true;
            }
        }

        public IEnumerable<float> GenerateRayOffsets()
        {
            var extent = controller.CharacterSize.StandingColliderSize.x / 2 - controller.CharacterSize.RayInset;
            var offsetAmount = extent / RAY_SIDE_COUNT;
            for (var i = 1; i < RAY_SIDE_COUNT + 1; i++)
            {
                yield return offsetAmount * i;
            }
        }

        private void ToggleGrounded(bool grounded)
        {
            this.grounded = grounded;
            if (grounded)
            {
                //GroundedChanged?.Invoke(true, _lastFrameY);
                controller.RB.gravityScale = 0;
                controller.SetVelocity(controller.FrameData.TrimmedVelocity);
                controller.ConstantForce.force = Vector2.zero;
                currentStepDownLength = controller.CharacterSize.StepHeight;
                //_canDash = true;
                controller.Jump.CoyoteUsable = true;
                controller.Jump.BufferedJumpUsable = true;
                controller.Jump.ResetAirJumps();
                controller.Slide.SlideJumping = false;
                SetColliderMode(ColliderMode.Standard);
            }
            else
            {
                //GroundedChanged?.Invoke(false, 0);
                controller.Jump.TimeLeftGrounded = controller.Time;
                controller.RB.gravityScale = controller.InitialGravityScale;
                SetColliderMode(ColliderMode.Airborne);
            }
        }

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