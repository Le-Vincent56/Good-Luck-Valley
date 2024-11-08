using GoodLuckValley.Player;
using GoodLuckValley.Player.Data;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler
{
    private enum ColliderMode
    {
        Standard,
        Crouching,
        Airborne
    }

    private PlayerController playerController;
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D airborneCollider;

    private const float SKIN_WIDTH = 0.02f;
    private const int RAY_SIDE_COUNT = 5;
    private RaycastHit2D groundHit;
    private bool grounded;
    private float currentStepDownLength;
    private float GrounderLength => playerController.CharacterSize.StepHeight + SKIN_WIDTH;

    private Vector2 RayPoint => playerController.FramePosition + playerController.Up * (playerController.CharacterSize.StepHeight + SKIN_WIDTH);

    public CollisionHandler(
        PlayerController playerController,
        BoxCollider2D boxCollider, 
        CapsuleCollider2D airborneCollider
    )
    {
        this.playerController = playerController;
        this.boxCollider = boxCollider;
        this.airborneCollider = airborneCollider;
    }

    private void Setup()
    {
        // Primary collider
        boxCollider = playerController.GetComponent<BoxCollider2D>();
        boxCollider.edgeRadius = CharacterSize.COLLIDER_EDGE_RADIUS;
        boxCollider.hideFlags = HideFlags.NotEditable;
        boxCollider.sharedMaterial = playerController.RB.sharedMaterial;
        boxCollider.enabled = true;

        // Airborne collider
        airborneCollider = playerController.GetComponent<CapsuleCollider2D>();
        airborneCollider.hideFlags = HideFlags.NotEditable;
        airborneCollider.size = new Vector2(playerController.CharacterSize.Width - SKIN_WIDTH * 2, playerController.CharacterSize.Height - SKIN_WIDTH * 2);
        airborneCollider.offset = new Vector2(0, playerController.CharacterSize.Height / 2);
        airborneCollider.sharedMaterial = playerController.RB.sharedMaterial;

        SetColliderMode(ColliderMode.Airborne);
    }

    private void CalculateCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        var isGroundedThisFrame = PerformRay(RayPoint);

        if (!isGroundedThisFrame)
        {
            foreach (float offset in GenerateRayOffsets())
            {
                isGroundedThisFrame = PerformRay(RayPoint + playerController.Right * offset) || PerformRay(RayPoint - playerController.Right * offset);
                if (isGroundedThisFrame) break;
            }
        }

        if (isGroundedThisFrame && !grounded) ToggleGrounded(true);
        else if (!isGroundedThisFrame && grounded) ToggleGrounded(false);

        Physics2D.queriesStartInColliders = playerController.CachedQueryMode;

        bool PerformRay(Vector2 point)
        {
            groundHit = Physics2D.Raycast(point, -playerController.Up, GrounderLength + currentStepDownLength, playerController.Stats.CollisionLayers);
            if (!groundHit) return false;

            if (Vector2.Angle(groundHit.normal, playerController.Up) > playerController.Stats.MaxWalkableSlope)
            {
                return false;
            }

            return true;
        }
    }

    private IEnumerable<float> GenerateRayOffsets()
    {
        var extent = playerController.CharacterSize.StandingColliderSize.x / 2 - playerController.CharacterSize.RayInset;
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
            //playerController.RB.gravityScale = 0;
            //SetVelocity(_trimmedFrameVelocity);
            //_constantForce.force = Vector2.zero;
            //currentStepDownLength = playerController.CharacterSize.StepHeight;
            //_canDash = true;
            //_coyoteUsable = true;
            //_bufferedJumpUsable = true;
            //ResetAirJumps();
            SetColliderMode(ColliderMode.Standard);
        }
        else
        {
            //GroundedChanged?.Invoke(false, 0);
            //_timeLeftGrounded = _time;
            //playerController.RB.gravityScale = GRAVITY_SCALE;
            SetColliderMode(ColliderMode.Airborne);
        }
    }

    private void SetColliderMode(ColliderMode mode)
    {
        airborneCollider.enabled = mode == ColliderMode.Airborne;

        switch (mode)
        {
            case ColliderMode.Standard:
                boxCollider.size = playerController.CharacterSize.StandingColliderSize;
                boxCollider.offset = playerController.CharacterSize.StandingColliderCenter;
                break;
            case ColliderMode.Crouching:
                boxCollider.size = playerController.CharacterSize.CrouchColliderSize;
                boxCollider.offset = playerController.CharacterSize.CrouchingColliderCenter;
                break;
            case ColliderMode.Airborne:
                break;
        }
    }
}
