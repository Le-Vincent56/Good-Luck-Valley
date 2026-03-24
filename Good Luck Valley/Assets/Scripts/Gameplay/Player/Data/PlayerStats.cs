using UnityEngine;

namespace GoodLuckValley.Gameplay.Player.Data
{
    /// <summary>
    /// Represents the configurable statistics and parameters for a player character
    /// in the "Good Luck Valley" game. This class is a ScriptableObject, allowing
    /// predefined settings to be shared across multiple instances of the player character.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Good Luck Valley/Player/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Collision")] 
        [SerializeField] private LayerMask collisionLayers;
        [SerializeField] private LayerMask ceilingLayers;
        [SerializeField] private LayerMask climbableLayer;

        [Header("Movement")] 
        [SerializeField] private float baseSpeed = 9f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float friction = 30f;
        [SerializeField] private float airFrictionMultiplier = 0.5f;
        [SerializeField] private float wallAirFrictionMultiplier = 0.225f;
        [SerializeField] private float directionCorrectionMultiplier = 3f;
        [SerializeField] private float maxWalkableSlope = 45f;
        [SerializeField] private float wallSlideMaxSpeed = 7f;
        [SerializeField] private float slidingMaxSpeed = 7f;
        [SerializeField] private float fallingMaxSpeed = 11f;

        [Header("Jump")] 
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpPower = 20f;
        [SerializeField] private float endJumpEarlyExtraForceMultiplier = 3f;
        [SerializeField] private int maxAirJumps = 1;
        [SerializeField] private float jumpClearanceTime = 0.25f;

        [Header("Bounce")] 
        [SerializeField] private float maxBouncePower = 10f;
        [SerializeField] private float minBouncePower = 10f;
        [SerializeField] private float bounceIgnoreDetectionTime = 0.2f;

        [Header("Walls")] 
        [SerializeField] private float wallJumpPowerX = 25f;
        [SerializeField] private float wallJumpPowerY = 15f;
        [SerializeField] private float wallClimbSpeed = 5f;
        [SerializeField] private float wallFallAcceleration = 20f;
        [SerializeField] private float wallCoyoteTime = 0.3f;
        [SerializeField] private float wallJumpTotalInputLossTime = 0.2f;
        [SerializeField] private float wallJumpInputLossReturnTime = 0.5f;
        [SerializeField] private float wallDetectorRange = 0.1f;
        [SerializeField] private float wallReattachCooldown = 0.2f;
        [SerializeField] private bool requireInputPush;

        [Header("Crawl")] 
        [SerializeField] private float crouchSlowDownTime = 0.5f;
        [SerializeField] private float crouchSpeedModifier = 0.5f;

        [Header("Gravity")] 
        [SerializeField] private float jumpGravityScale = 2.8f;
        [SerializeField] private float fallGravityScale = 1.5f;
        [SerializeField] private float fastFallMultiplier = 2f;
        [SerializeField] private float wallSlideGravityScale = 1.3f;
        [SerializeField] private float bounceGravityScale = 2.2f;
        [SerializeField] private float slideGravityScale = 0.75f;

        [Header("External Forces")] 
        [SerializeField] private float externalVelocityDecayRate = 0.1f;

        [Header("Input")] 
        [SerializeField] private float verticalDeadZoneThreshold = 0.3f;
        [SerializeField] private float horizontalDeadZoneThreshold = 0.1f;

        public LayerMask CollisionLayers => collisionLayers;
        public LayerMask CeilingLayers => ceilingLayers;
        public LayerMask ClimbableLayer => climbableLayer;
        public float BaseSpeed => baseSpeed;
        public float Acceleration => acceleration;
        public float Friction => friction;
        public float AirFrictionMultiplier => airFrictionMultiplier;
        public float WallAirFrictionMultiplier => wallAirFrictionMultiplier;
        public float DirectionCorrectionMultiplier => directionCorrectionMultiplier;
        public float MaxWalkableSlope => maxWalkableSlope;
        public float WallSlideMaxSpeed => wallSlideMaxSpeed;
        public float SlidingMaxSpeed => slidingMaxSpeed;
        public float FallingMaxSpeed => fallingMaxSpeed;
        public float CoyoteTime => coyoteTime;
        public float JumpPower => jumpPower;
        public float EndJumpEarlyExtraForceMultiplier => endJumpEarlyExtraForceMultiplier;
        public int MaxAirJumps => maxAirJumps;
        public float JumpClearanceTime => jumpClearanceTime;
        public float MaxBouncePower => maxBouncePower;
        public float MinBouncePower => minBouncePower;
        public float BounceIgnoreDetectionTime => bounceIgnoreDetectionTime;
        public float WallJumpPowerX => wallJumpPowerX;
        public float WallJumpPowerY => wallJumpPowerY;
        public float WallClimbSpeed => wallClimbSpeed;
        public float WallFallAcceleration => wallFallAcceleration;
        public float WallCoyoteTime => wallCoyoteTime;
        public float WallJumpTotalInputLossTime => wallJumpTotalInputLossTime;
        public float WallJumpInputLossReturnTime => wallJumpInputLossReturnTime;
        public float WallDetectorRange => wallDetectorRange;
        public float WallReattachCooldown => wallReattachCooldown;
        public bool RequireInputPush => requireInputPush;
        public float CrouchSlowDownTime => crouchSlowDownTime;
        public float CrouchSpeedModifier => crouchSpeedModifier;
        public float JumpGravityScale => jumpGravityScale;
        public float FallGravityScale => fallGravityScale;
        public float FastFallMultiplier => fastFallMultiplier;
        public float WallSlideGravityScale => wallSlideGravityScale;
        public float BounceGravityScale => bounceGravityScale;
        public float SlideGravityScale => slideGravityScale;
        public float ExternalVelocityDecayRate => externalVelocityDecayRate;
        public float VerticalDeadZoneThreshold => verticalDeadZoneThreshold;
        public float HorizontalDeadZoneThreshold => horizontalDeadZoneThreshold;
    }
}