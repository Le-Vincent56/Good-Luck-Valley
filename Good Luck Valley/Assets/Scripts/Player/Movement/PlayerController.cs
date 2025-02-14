using GoodLuckValley.Input;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Player;
using GoodLuckValley.Extensions.GameObjects;
using GoodLuckValley.Player.Data;
using UnityEngine;
using GoodLuckValley.World.Physics;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Potentiates;
using GoodLuckValley.Audio;
using System.Runtime.CompilerServices;
using GoodLuckValley.Particles;


namespace GoodLuckValley.Player.Movement
{
    public class PlayerController : MonoBehaviour, IPhysicsObject
    {
        [Header("References")]
        [SerializeField] private GameInputReader input;
        [SerializeField] private PlayerStats stats;
        [SerializeField] private Transform followTransform;
        private Rigidbody2D rb;
        private ConstantForce2D constForce;
        private BoxCollider2D boxCollider;
        private CapsuleCollider2D airborneCollider;
        private PotentiateHandler potentiateHandler;
        private PlayerStateMachine stateMachine;
        private LayerDetection layerDetection;
        private ParticleController particles;

        [Header("Movement Components")]
        [SerializeField] private bool active;
        [SerializeField] private bool forcedMove;
        [SerializeField] private int forcedMoveDirection;
        [SerializeField] private CollisionHandler collisionHandler;
        private FrameData frameData;
        [SerializeField] private PlayerJump jump;
        [SerializeField] private PlayerCrawl crawl;
        [SerializeField] private PlayerBounce bounce;
        [SerializeField] private PlayerWallJump wallJump;

        private GeneratedCharacterSize characterSize;
        private bool cachedQueryMode;
        private bool cachedQueryTriggers;
        public const float GRAVITY_SCALE = 1f;
        private float extraConstantGravity;

        private float delta;
        private float time;

        private Vector2 direction;

        private Vector2 immediateMove;
        private Vector2 decayingTransientVelocity;
        private Vector2 frameSpeedModifier = Vector2.one;
        private Vector2 currentFrameSpeedModifier = Vector2.one;
        private const float SLOPE_ANGLE_FOR_EXACT_MOVEMENT = 0.7f;

        [SerializeField] private bool debug;

        public GameInputReader Input { get => input; }
        public PlayerStats Stats { get => stats; }
        public Transform FollowTransform { get => followTransform; }
        public Rigidbody2D RB { get => rb; }
        public ConstantForce2D ConstantForce { get => constForce; }

        public bool Active { get => active; set => active = value; }
        public bool ForcedMove { get => forcedMove; set => forcedMove = value; }
        public int ForcedMoveDirection { get => forcedMoveDirection; set => forcedMoveDirection = value; }
        public CollisionHandler Collisions { get => collisionHandler; }
        public FrameData FrameData { get => frameData; }
        public PlayerJump Jump { get => jump; }
        public PlayerCrawl Crawl { get => crawl; }
        public PlayerBounce Bounce { get => bounce; }
        public PlayerWallJump WallJump { get => wallJump; }
        public ParticleController Particles { get => particles; }

        public GeneratedCharacterSize CharacterSize { get => characterSize; }
        public bool CachedQueryMode { get => cachedQueryMode; }
        public float InitialGravityScale { get => GRAVITY_SCALE; }

        public float Delta { get => delta; }
        public float Time { get => time; }

        public Vector2 Velocity { get; set; }
        public Vector2 ImmediateMove { get => immediateMove; }
        public Vector2 DecayingTransientVelocity { get => decayingTransientVelocity; set => decayingTransientVelocity = value; }
        public Vector2 Direction { get => direction; set => direction = value; }

        public Vector2 Up { get; set; }
        public Vector2 Right { get; set; }
        public Vector2 Down { get; set; }
        public Vector2 Left { get; set; }

        private EventBinding<ForcePlayerMove> onForcePlayerMove;
        private EventBinding<PlacePlayer> onPlacePlayer;
        private EventBinding<ActivatePlayer> onActivatePlayer;
        private EventBinding<DeactivatePlayer> onDeactivatePlayer;

        private void Awake()
        {
            // Get the components
            constForce = GameObjectExtensions.GetOrAdd<ConstantForce2D>(gameObject);
            potentiateHandler = GameObjectExtensions.GetOrAdd<PotentiateHandler>(gameObject);
            stateMachine = GameObjectExtensions.GetOrAdd<PlayerStateMachine>(gameObject);

            // Set variables
            active = true;
            forcedMove = false;

            // Set up the player controller
            Setup();
        }

        private void OnEnable()
        {
            onForcePlayerMove = new EventBinding<ForcePlayerMove>(SetForcedMove);
            EventBus<ForcePlayerMove>.Register(onForcePlayerMove);

            onPlacePlayer = new EventBinding<PlacePlayer>(PlacePlayerAtPosition);
            EventBus<PlacePlayer>.Register(onPlacePlayer);

            onActivatePlayer = new EventBinding<ActivatePlayer>(Activate);
            EventBus<ActivatePlayer>.Register(onActivatePlayer);

            onDeactivatePlayer = new EventBinding<DeactivatePlayer>(Deactivate);
            EventBus<DeactivatePlayer>.Register(onDeactivatePlayer);
        }

        private void OnDisable()
        {
            EventBus<ForcePlayerMove>.Deregister(onForcePlayerMove);
            EventBus<PlacePlayer>.Deregister(onPlacePlayer);
            EventBus<ActivatePlayer>.Deregister(onActivatePlayer);
            EventBus<DeactivatePlayer>.Deregister(onDeactivatePlayer);
        }

        public void OnValidate() => Setup();

        private void Start()
        {
            // Set the player
            PhysicsOrchestrator orchestrator = ServiceLocator.ForSceneOf(this).Get<PhysicsOrchestrator>();
            orchestrator.SetPlayer(this);
        }

        /// <summary>
        /// Set up the PlayerController
        /// </summary>
        private void Setup()
        {
            // Generate the character size
            characterSize = stats.CharacterSize.GenerateCharacterSize();
            cachedQueryMode = Physics2D.queriesStartInColliders;

            // Initialize the Rigidbody2D
            rb = GetComponent<Rigidbody2D>();
            rb.hideFlags = HideFlags.NotEditable;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            // Get components
            particles = GetComponentInChildren<ParticleController>();

            // Get the Colliders
            boxCollider = GetComponent<BoxCollider2D>();
            airborneCollider = GetComponent<CapsuleCollider2D>();

            frameData = new FrameData(this);

            // Initialize the Collision Handler
            collisionHandler = new CollisionHandler(this, boxCollider, airborneCollider);
            collisionHandler.Setup();

            // Initialize movement components
            jump = new PlayerJump(this, potentiateHandler);
            crawl = new PlayerCrawl(this);
            bounce = new PlayerBounce(this);
            wallJump = new PlayerWallJump(this);

            // Get the layer detection component
            layerDetection = GetComponent<LayerDetection>();

            input.Enable();
        }

        public void SetCharacterSize() => stats.CharacterSize.GenerateCharacterSize();

        public void TickUpdate(float delta, float time)
        {
            // Set layer detection
            layerDetection.SetGroundLayer(collisionHandler.LastGroundLayer);
            layerDetection.SetWallLayer(wallJump.LastWallLayer);
            layerDetection.SetWallDirection(wallJump.WallDirectionThisFrame);

            // Update the state machine
            stateMachine.TickUpdate();

            // Exit case - the PlayerController is not active
            if (!active) return;

            // Set time variables
            this.delta = delta;
            this.time = time;

            // Gather input for the frame
            frameData.GatherInput(input);
        }

        public void TickFixedUpdate(float delta)
        {
            // Update the state machine
            stateMachine.TickFixedUpdate();

            //if (debug) DebugMovement(true);

            // Set time variables
            this.delta = delta;

            // Remove transient velocity
            RemoveTransientVelocity();

            // Set frame data
            frameData.Set();

            // Calculate collisions
            if (bounce.CanDetectGround)
                collisionHandler.CalculateCollisions();

            // Calculate the direction of movement
            CalculateDirection();

            // Calculate movement components
            wallJump.CalculateWalls();
            jump.CalculateJump();
            bounce.CalculateBounce();

            // Move
            TraceGround();
            TraceWall();
            Move();

            // Calculate crouch
            crawl.CalculateCrawl();

            // Clean the frame data
            frameData.Clean();

            //if (debug) DebugMovement(false);
        }

        private void DebugMovement(bool before)
        {
            string beforeText = before ? "Before" : "After";

            Debug.Log($"Movement Debug ({beforeText})): " +
                $"\nVelocity: {Velocity}" +
                $"\nLinear Velocity: {rb.linearVelocity}" +
                $"\nDecaying Transient Velocity: {DecayingTransientVelocity}" +
                $"\nTransient Velocity: {FrameData.TransientVelocity}" +
                $"\nExtra Constant Gravity: {extraConstantGravity}" +
                $"\nForce to Apply: {FrameData.ForceToApply}" +
                $"\nAdditional Frame Velocities: {FrameData.AdditionalFrameVelocities()}" +
                $"\nConstant Force: {constForce.force}"
            );
        }

        /// <summary>
        /// Calculate the direction of movement
        /// </summary>
        private void CalculateDirection()
        {
            // Get the direction of movement
            direction = !active 
                ? new Vector2(0, 0)
                : !forcedMove 
                    ? new Vector2(frameData.Input.Move.x, 0) 
                    : new Vector2(forcedMoveDirection, 0);

            // Check if grounded
            if (collisionHandler.Grounded)
            {
                // Check for an angle
                float angle = Vector2.Angle(collisionHandler.GroundNormal, Up);

                // Check if the angle is less than the max walkable slope
                if (angle < Stats.MaxWalkableSlope) 
                    // Set the angle to walk on the slope
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
            rb.linearVelocity = newVelocity;
            Velocity = newVelocity;
        }

        /// <summary>
        /// Set the gravity scale of the PlayerController
        /// </summary>
        public void SetGravityScale(float gravityScale) => rb.gravityScale = gravityScale;

        /// <summary>
        /// Move the player
        /// </summary>
        private void Move()
        {
            // Check if there is force to apply this frame
            if (FrameData.ForceToApply != Vector2.zero)
            {
                // Add the additional frame velocities and force
                rb.linearVelocity += FrameData.AdditionalFrameVelocities();
                rb.AddForce(FrameData.ForceToApply * rb.mass, ForceMode2D.Impulse);

                return;
            }

            // Check if on a wall
            if (wallJump.IsOnWall)
            {
                // Zero out the constant force
                constForce.force = Vector2.zero;

                // Create a container for the wall velocity
                float wallVelocity;

                // Check if holding downwards
                if (FrameData.Input.Move.y < 0)
                    // Set wall velocity to the wall climb speed
                    wallVelocity = FrameData.Input.Move.y * Stats.WallClimbSpeed;
                else
                    // Otherwise, set a fall speed
                    wallVelocity = Mathf.MoveTowards(Mathf.Min(Velocity.y, 0), -Stats.WallClimbSpeed, Stats.WallFallAcceleration * delta);

                // Set the velocity
                SetVelocity(new Vector2(rb.linearVelocity.x, wallVelocity));

                return;
            }

            // Calculate the extra force
            Vector2 extraForce = new Vector2(
                0,
                Collisions.Grounded
                    ? 0
                    : (Jump.EndedJumpEarly && Velocity.y > 0 && !Bounce.Bouncing && !WallJump.FromWallJump)
                          ? -Stats.EndJumpEarlyExtraForceMultiplier
                          : 0
             );

            // Add the extra force to the ConstantForce2D
            constForce.force = extraForce * rb.mass;

            // Calculate the target speed based on if there's input this frame
            float targetSpeed = FrameData.HasInput || forcedMove ? Stats.BaseSpeed : 0;

            // Check if crouching
            if (crawl.Crawling)
            {
                // Slow the player down to the crouch speed
                float crawlPoint = Mathf.InverseLerp(0, Stats.CrouchSlowDownTime, time - crawl.TimeStartedCrawling);
                targetSpeed *= Mathf.Lerp(1, Stats.CrouchSpeedModifier, crawlPoint);
            }

            // Calculate the step of movement
            float step = FrameData.HasInput || forcedMove 
                ? Stats.Acceleration 
                : Stats.Friction;

            // Get the x-direction of movement
            Vector2 xDirection = FrameData.HasInput || forcedMove
                ? direction 
                : Velocity.normalized;

            // Check if the trimmed velocity and the direction are moving in opposite directions
            if (Vector3.Dot(FrameData.TrimmedVelocity, direction) < 0)
                // If so, apply the correction multiplier
                step *= Stats.DirectionCorrectionMultiplier;

            Vector2 newVelocity;
            step *= delta;

            // Check if grounded
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

                // Check if the wall jump input nerf point is less than 1 and if the directions of the wall jump match
                if (wallJump.WallJumpInputNerfPoint < 1 && (int)Mathf.Sign(xDirection.x) == (int)Mathf.Sign(wallJump.WallDirectionForJump))
                {
                    // Check if the time is less than the time to return wall input
                    if (time < wallJump.ReturnWallInputLossAfter) 
                        // Set the direction to the opposite of the wall direction for jumping
                        xDirection.x = -wallJump.WallDirectionForJump;
                    // Otherwise
                    else 
                        // Multiply the x-drection by the wall jump input nerf point
                        xDirection.x *= wallJump.WallJumpInputNerfPoint;
                }

                // Set the new velocity
                float targetX = Mathf.MoveTowards(FrameData.TrimmedVelocity.x, xDirection.x * targetSpeed, step);
                newVelocity = new Vector2(targetX, rb.linearVelocity.y);
            }

            // Set the new velocity with any additional velocities
            SetVelocity((newVelocity + FrameData.AdditionalFrameVelocities()) * currentFrameSpeedModifier);
        }

        /// <summary>
        /// Trace the ground underneath the Player
        /// </summary>
        private void TraceGround()
        {
            // Check if grounded and if not within jump clearance
            if(Collisions.Grounded && !Jump.IsWithinJumpClearance)
            {
                // Get the distance from the ground
                float distanceFromGround = characterSize.StepHeight - Collisions.GroundHit.distance;

                // Check if there's still distance to travel
                if(distanceFromGround != 0)
                {
                    // Get the movement vector to travel the distance
                    Vector2 requiredMove = Vector2.zero;
                    requiredMove.y += distanceFromGround;

                    // Check if using velocity as the PositionCorrectionMode
                    if (Stats.PositionCorrectionMode is PositionCorrectionMode.Velocity)
                        // Add the correction to the transient velocity
                        FrameData.TransientVelocity = requiredMove / delta;
                    else
                        // Move immediately
                        immediateMove = requiredMove;
                }
            }
        }

        /// <summary>
        /// Trace the wall next to the player
        /// </summary>
        private void TraceWall()
        {
            if(WallJump.StickToWall && !Jump.IsWithinJumpClearance)
            {
                // Get the distance from the wall
                float distanceFromWall = (stats.WallDetectorRange - WallJump.WallHit.distance) * wallJump.WallDirectionThisFrame;

                // Check if there's still distance to travel
                if(distanceFromWall != 0)
                {
                    // Get the movement vector to travel the distance
                    Vector2 requiredMove = Vector2.zero;
                    requiredMove.x += distanceFromWall;

                    // Check if using velocity as the PositionCorrectionMode
                    if (Stats.PositionCorrectionMode is PositionCorrectionMode.Velocity)
                        // Add the correction to the transient velocity
                        FrameData.TransientVelocity = requiredMove / delta;
                    else
                        // Move immediately
                        immediateMove = requiredMove;
                }
            }
        }

        /// <summary>
        /// Remove transient velocities from the last frame
        /// </summary>
        private void RemoveTransientVelocity()
        {
            Vector2 currentVelocity = rb.linearVelocity;
            Vector2 velocityBeforeReduction = currentVelocity;

            // Subtract the current velocity from the previous frame's transient velocity
            currentVelocity -= FrameData.PreviousTotalTransientVelocity;

            // Set the current velocity
            SetVelocity(currentVelocity);

            // Zero out frame transient velocities
            FrameData.TransientVelocity = Vector2.zero;
            FrameData.PreviousTotalTransientVelocity = Vector2.zero;

            // Get the decay factor
            float decay = Stats.Friction * Stats.AirFrictionMultiplier * Stats.ExternalVelocityDecayRate;

            if ((velocityBeforeReduction.x < 0 && decayingTransientVelocity.x < velocityBeforeReduction.x) ||
                (velocityBeforeReduction.x > 0 && decayingTransientVelocity.x > velocityBeforeReduction.x) ||
                (velocityBeforeReduction.y < 0 && decayingTransientVelocity.y < velocityBeforeReduction.y) ||
                (velocityBeforeReduction.y > 0 && decayingTransientVelocity.y > velocityBeforeReduction.y)) decay *= 5;

            // Set the decaying transient velocity
            decayingTransientVelocity = Vector2.MoveTowards(decayingTransientVelocity, Vector2.zero, decay * delta);

            // Zero out immediate move
            immediateMove = Vector2.zero;
        }

        /// <summary>
        /// Set the variables for a forced Player move
        /// </summary>
        private void SetForcedMove(ForcePlayerMove eventData)
        {
            forcedMove = eventData.ForcedMove;
            forcedMoveDirection = eventData.ForcedMoveDirection;
        }

        /// <summary>
        /// Activate the player
        /// </summary>
        private void Activate()
        {
            active = true;
            input.Enable();
        }

        /// <summary>
        /// Deactivate the player
        /// </summary>
        private void Deactivate()
        {
            active = false;
            input.Disable();
        }

        /// <summary>
        /// Event callback top place the Player at a position
        /// </summary>
        private void PlacePlayerAtPosition(PlacePlayer eventData) => PlacePlayerAtPosition(eventData.Position);

        /// <summary>
        /// Place the Player at a position explicitly
        /// </summary>
        public void PlacePlayerAtPosition(Vector3 position)
        {
            Vector3 worldPosition = transform.parent.TransformPoint(position);

            transform.position = worldPosition;
            rb.position = worldPosition;
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;

            Vector2 pos = (Vector2)transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos + Vector2.up * characterSize.Height / 2f, new Vector3(characterSize.Width, characterSize.Height));

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(pos + (Vector2)wallJump.WallDetectionBounds.center, wallJump.WallDetectionBounds.size);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(pos + Vector2.up * characterSize.CrouchingHeight / 2f, new Vector3(characterSize.Width, characterSize.CrouchingHeight));

            Gizmos.color = Color.magenta;
            Vector2 rayStart = pos + Vector2.up * characterSize.StepHeight;
            Vector3 rayDir = Vector3.down * characterSize.StepHeight;
            Gizmos.DrawRay(rayStart, rayDir);
            foreach (float offset in Collisions.GenerateRayOffsets())
            {
                Gizmos.DrawRay(rayStart + Vector2.right * offset, rayDir);
                Gizmos.DrawRay(rayStart + Vector2.left * offset, rayDir);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(collisionHandler.RayPoint, 0.5f);

            Gizmos.color = Color.black;
            Gizmos.DrawRay(Collisions.RayPoint, Vector3.right);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(pos + Vector2.up * characterSize.Height / 2f, direction);

            if (crawl.Pos == null || crawl.Size == null) return;

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(crawl.Pos, crawl.Size);
        }
    }
}