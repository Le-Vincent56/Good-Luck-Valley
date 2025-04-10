using GoodLuckValley.Player.Movement;
using System;
using UnityEngine;

namespace GoodLuckValley.Player.Data
{
    [Serializable]
    public enum PositionCorrectionMode
    {
        Velocity,
        Immediate
    }

    [CreateAssetMenu(fileName = "Player Stats", menuName = "Data/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        // Setup
        [Header("Setup")] public LayerMask PlayerLayer;
        public LayerMask CollisionLayers;
        public LayerMask CeilingLayers;
        public LayerMask TraceableLayers;
        public CharacterSize CharacterSize;

        // Controller Setup
        [Header("Controller Setup"), Space] public float VerticalDeadZoneThreshold = 0.3f;
        public double HorizontalDeadZoneThreshold = 0.1f;

        [Tooltip("Velocity = smoother, but can be occasionally unreliable on jagged terrain. Immediate = Occasionally jittery, but stable")]
        public PositionCorrectionMode PositionCorrectionMode = PositionCorrectionMode.Velocity;

        // Movement
        [Header("Movement"), Space] 
        public float BaseSpeed = 9;
        public float Acceleration = 50;
        public float Friction = 30;
        public float AirFrictionMultiplier = 0.5f;
        public float WallAirFrictionMultiplier = 0.225f;
        public float DirectionCorrectionMultiplier = 3f;
        public float MaxWalkableSlope = 45;
        public float WallSlideMaxSpeed = 7f;
        public float SlidingMaxSpeed = 7f;
        public float FallingMaxSpeed = 11f;

        [Header("Gravity")]
        public float FallGravityScale = 1.5f;
        public float JumpGravityScale = 2.8f;
        public float BounceGravityScale = 2.2f;
        public float WallSlideGravityScale = 1.3f;
        public float TimeWarpGravityScale = 0.5f;
        public float SlideGravityScale = 0.75f;

        [Header("Fall"), Space]
        public float FastFallMultiplier = 2f;
        public float SlowFallMultiplier = 0.75f;

        [Header("Time Warp"), Space]
        public Vector2 TimeWarpSpeedModifier = new Vector2(0.5f, 0.75f);

        // Jump
        [Header("Jump"), Space] 
        public float BufferedJumpTime = 0.15f;
        public float CoyoteTime = 0.15f;
        public float JumpPower = 20;
        public float EndJumpEarlyExtraForceMultiplier = 3;
        public int MaxAirJumps = 1;

        [Header("Bounce"), Space]
        public float MaxBouncePower = 10f;
        public float MinBouncePower = 10f;
        public float BounceIgnoreDetectionTime = 0.2f;

        // Dash
        [Header("Dash"), Space] 
        public bool AllowDash = true;
        public float DashVelocity = 50;
        public float DashDuration = 0.2f;
        public float DashCooldown = 1.5f;
        public float DashEndHorizontalMultiplier = 0.5f;

        // Dash
        [Header("Crouch"), Space] 
        public bool AllowCrouching;
        public float CrouchSlowDownTime = 0.5f;
        public float CrouchSpeedModifier = 0.5f;

        // Walls
        [Header("Walls"), Space] 
        public bool AllowWalls;
        public LayerMask ClimbableLayer;
        public float WallJumpTotalInputLossTime = 0.2f;
        public float WallJumpInputLossReturnTime = 0.5f;
        public bool RequireInputPush;
        public Vector2 WallJumpPower = new(25, 15);
        public Vector2 WallPushPower = new(15, 10);
        public float WallStickPower = 1f;
        public float WallClimbSpeed = 5;
        public float WallFallAcceleration = 20;
        public float WallPopForce = 10;
        public float WallCoyoteTime = 0.3f;
        public float WallDetectorRange = 0.1f;

        // Ladders
        [Header("Ladders"), Space] 
        public bool AllowLadders;
        public double LadderCooldownTime = 0.15f;
        public bool AutoAttachToLadders = true;
        public bool SnapToLadders = true;
        public LayerMask LadderLayer;
        public float LadderSnapTime = 0.02f;
        public float LadderPopForce = 10;
        public float LadderClimbSpeed = 8;
        public float LadderSlideSpeed = 12;
        public float LadderShimmySpeedMultiplier = 0.5f;

        // Moving Platforms
        [Header("Moving Platforms"), Space] 
        public float NegativeYVelocityNegation = 0.2f;
        public float ExternalVelocityDecayRate = 0.1f;

        private void OnValidate()
        {
            PlayerController[] potentialPlayer = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (PlayerController player in potentialPlayer)
            {
                player.OnValidate();
            }
        }
    }
}