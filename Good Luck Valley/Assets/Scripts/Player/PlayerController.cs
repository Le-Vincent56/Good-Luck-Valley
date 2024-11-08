using GoodLuckValley.Input;
using GoodLuckValley.Extensions.GameObjects;
using GoodLuckValley.Player.Data;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using GoodLuckValley.World.Physics;
using GoodLuckValley.Patterns.ServiceLocator;

namespace GoodLuckValley.Player
{
    public class PlayerController : SerializedMonoBehaviour, IPhysicsObject
    {
        // References
        [SerializeField] private GameInputReader input;
        [SerializeField] private PlayerStats stats;
        private Rigidbody2D rb;
        private ConstantForce2D constForce;
        private BoxCollider2D boxCollider;
        private CapsuleCollider2D airborneCollider;

        // Movement components
        private CollisionHandler collisionHandler;
        private FrameData frameData;
        private PlayerJump jump;
        private PlayerCrouch crouch;

        [SerializeField] private GeneratedCharacterSize characterSize;
        private bool cachedQueryMode;
        private bool cachedQueryTriggers;
        public const float GRAVITY_SCALE = 1;

        private float delta;
        private float time;

        private Vector2 direction;

        private Vector2 immediateMove;
        private Vector2 decayingTransientVelocity;
        private Vector2 frameSpeedModifier = Vector2.one;
        private Vector2 currentFrameSpeedModifier = Vector2.one;
        private const float SLOPE_ANGLE_FOR_EXACT_MOVEMENT = 0.7f;

        [SerializeField] private bool debug;

        public PlayerStats Stats { get => stats; }
        public Rigidbody2D RB { get => rb; }
        public ConstantForce2D ConstantForce { get => constForce; }

        public CollisionHandler Collisions { get => collisionHandler; }
        public FrameData FrameData { get => frameData; }
        public PlayerJump Jump { get => jump; }
        public PlayerCrouch Crouch { get => crouch; }

        public GeneratedCharacterSize CharacterSize { get => characterSize; }
        public bool CachedQueryMode { get => cachedQueryMode; }
        public float GravityScale { get => GRAVITY_SCALE; }

        public float Time { get => time; }

        public Vector2 Velocity { get; set; }
        public Vector2 ImmediateMove { get => immediateMove; }
        public Vector2 DecayingTransientVelocity { get => decayingTransientVelocity; }
        public Vector2 Direction { get => direction; set => direction = value; }

        public Vector2 Up { get; set; }
        public Vector2 Right { get; set; }
        public Vector2 Down { get; set; }
        public Vector2 Left { get; set; }

        public event Action<PlayerJump.JumpType> Jumped;

        private void Awake()
        {
            // Get the constant force component
            constForce = GameObjectExtensions.GetOrAdd<ConstantForce2D>(gameObject);

            // Set up the player controller
            Setup();
        }

        public void OnValidate() => Setup();

        private void Start()
        {
            // Set the player
            PhysicsOrchestrator orchestrator = ServiceLocator.ForSceneOf(this).Get<PhysicsOrchestrator>();
            orchestrator.SetPlayer(this);
        }

        private void Setup()
        {
            // Generate the character size
            characterSize = stats.CharacterSize.GenerateCharacterSize();
            cachedQueryMode = Physics2D.queriesStartInColliders;

            //_wallDetectionBounds = new Bounds(
            //    new Vector3(0, characterSize.Height / 2),
            //    new Vector3(characterSize.StandingColliderSize.x + CharacterSize.COLLIDER_EDGE_RADIUS * 2 + Stats.WallDetectorRange, characterSize.Height - 0.1f));

            // Initialize the Rigidbody2D
            rb = GetComponent<Rigidbody2D>();
            rb.hideFlags = HideFlags.NotEditable;

            // Get the Colliders
            boxCollider = GetComponent<BoxCollider2D>();
            airborneCollider = GetComponent<CapsuleCollider2D>();

            frameData ??= new FrameData(this);

            // Initialize the Collision Handler
            collisionHandler ??= new CollisionHandler(this, boxCollider, airborneCollider);
            collisionHandler.Setup();

            // Initialize movement components
            jump ??= new PlayerJump(this);
            crouch ??= new PlayerCrouch(this);

            input.Enable();
        }

        public void TickUpdate(float delta, float time)
        {
            // Set time variables
            this.delta = delta;
            this.time = time;

            // Gather input for the frame
            frameData.GatherInput(input);
        }

        public void TickFixedUpdate(float delta)
        {
            // Set time variables
            this.delta = delta;

            // Set frame data
            frameData.Set();

            // Calculate collisions
            collisionHandler.CalculateCollisions();

            // Calculate the direction of movement
            CalculateDirection();

            // Calculate movement components
            jump.CalculateJump();

            // Move
            Move();

            // Calculate crouch
            crouch.CalculateCrouch();

            // Clean the frame data
            frameData.Clean();
        }

        private void CalculateDirection()
        {
            // Get the direction of movement
            direction = new Vector2(frameData.Input.Move.x, 0);

            // Check if grounded
            if (collisionHandler.Grounded)
            {
                // Check for an angle
                float angle = Vector2.Angle(collisionHandler.GroundNormal, Up);

                // Check if the angle is less than the max walkable slope
                if (angle < Stats.MaxWalkableSlope) 
                    // Set the direction to slide down it
                    direction.y = direction.x * -collisionHandler.GroundNormal.x / collisionHandler.GroundNormal.y;
            }

            // Normalize the direction
            direction = direction.normalized;
        }

        /// <summary>
        /// Set the PlayerController's velocity
        /// </summary>
        public void SetVelocity(Vector2 newVelocity)
        {
            rb.velocity = newVelocity;
            Velocity = newVelocity;
        }

        private void Move()
        {
            // Check if there is force to apply this frame
            if(FrameData.ForceToApply != Vector2.zero)
            {
                // Add the additional frame velocities and force
                rb.velocity += FrameData.AdditionalFrameVelocities();
                rb.AddForce(FrameData.ForceToApply * rb.mass, ForceMode2D.Impulse);

                return;
            }

            // Store a multiplier if the jump ended early
            float endJumpEarlyMult = Jump.EndedJumpEarly && Velocity.y > 0
                ? Stats.EndJumpEarlyExtraForceMultiplier
                : 1;

            // Calculate the extra force
            Vector2 extraForce = new Vector2(0,
                Collisions.Grounded ? 0 : -Stats.ExtraConstantGravity * endJumpEarlyMult
            );

            // Add the extra force to the ConstantForce2D
            constForce.force = extraForce * rb.mass;

            // Calculate the target speed based on if there's input this frame
            float targetSpeed = FrameData.HasInput ? Stats.BaseSpeed : 0;

            // Check if crouching
            if(crouch.Crouching)
            {
                // Slow the player down to the crouch speed
                float crouchPoint = Mathf.InverseLerp(0, Stats.CrouchSlowDownTime, time - crouch.TimeStartedCrouching);
                targetSpeed *= Mathf.Lerp(1, Stats.CrouchSpeedModifier, crouchPoint);
            }

            float step = FrameData.HasInput ? Stats.Acceleration : Stats.Friction;

            Vector2 xDirection = FrameData.HasInput ? direction : Velocity.normalized;

            if (Vector3.Dot(FrameData.TrimmedVelocity, direction) < 0) 
                step *= Stats.DirectionCorrectionMultiplier;

            Vector2 newVelocity;
            step *= delta;

            if(Collisions.Grounded)
            {
                // Calculate the speed of movement
                float speed = Mathf.MoveTowards(Velocity.magnitude, targetSpeed, step);

                // Blend the two approaches
                Vector2 targetVelocity = xDirection * speed;

                // Calculate the new speed based on the current and target speeds
                float newSpeed = Mathf.MoveTowards(Velocity.magnitude, targetVelocity.magnitude, step);

                Vector2 smoothed = Vector2.MoveTowards(Velocity, targetVelocity, step);
                Vector2 direct = targetVelocity.normalized * newSpeed;
                float slopePoint = Mathf.InverseLerp(0, SLOPE_ANGLE_FOR_EXACT_MOVEMENT, Mathf.Abs(direction.y));

                // Calculate the blended velocity
                newVelocity = Vector2.Lerp(smoothed, direct, slopePoint);
            } 
            else
            {
                step *= Stats.AirFrictionMultiplier;

                //if (_wallJumpInputNerfPoint < 1 && (int)Mathf.Sign(xDir.x) == (int)Mathf.Sign(_wallDirectionForJump))
                //{
                //    if (_time < _returnWallInputLossAfter) xDir.x = -_wallDirectionForJump;
                //    else xDir.x *= _wallJumpInputNerfPoint;
                //}

                float targetX = Mathf.MoveTowards(FrameData.TrimmedVelocity.x, xDirection.x * targetSpeed, step);
                newVelocity = new Vector2(targetX, rb.velocity.y);
            }

            SetVelocity((newVelocity + FrameData.AdditionalFrameVelocities()) * currentFrameSpeedModifier);
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;

            var pos = (Vector2)transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos + Vector2.up * characterSize.Height / 2, new Vector3(characterSize.Width, characterSize.Height));
            Gizmos.color = Color.magenta;

            var rayStart = pos + Vector2.up * characterSize.StepHeight;
            var rayDir = Vector3.down * characterSize.StepHeight;
            Gizmos.DrawRay(rayStart, rayDir);
            foreach (float offset in Collisions.GenerateRayOffsets())
            {
                Gizmos.DrawRay(rayStart + Vector2.right * offset, rayDir);
                Gizmos.DrawRay(rayStart + Vector2.left * offset, rayDir);
            }

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireCube(pos + (Vector2)_wallDetectionBounds.center, _wallDetectionBounds.size);


            Gizmos.color = Color.black;
            Gizmos.DrawRay(Collisions.RayPoint, Vector3.right);
        }
    }
}