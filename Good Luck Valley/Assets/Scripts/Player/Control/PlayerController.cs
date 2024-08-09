using GHoodLuckValley.Player.Data;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Player.Input;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;
using GoodLuckValley.Player.States;
using GoodLuckValley.Entity;
using GoodLuckValley.Events;
using GoodLuckValley.Cameras;
using GoodLuckValley.Patterns.Blackboard;   
using GoodLuckValley.Audio.SFX;
using System.Collections;
using GoodLuckValley.VFX.Particles.Controllers;

namespace GoodLuckValley.Player.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onLearnControl;
        [SerializeField] private GameEvent onLearnChainBounce;
        [SerializeField] private GameEvent onResetBounce;
        [SerializeField] private GameEvent onWallJumpInput;
        [SerializeField] private GameEvent onSendPlayerTransform;
        [SerializeField] private GameEvent onSendPlayerController;
        [SerializeField] private GameEvent onPlayerTurn;
        [SerializeField] private GameEvent onSetCanPeek;
        [SerializeField] private GameEvent onPositionChange;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private InputReader input;
        [SerializeField] private DynamicCollisionHandler collisionHandler;
        [SerializeField] private PlayerData data;
        [SerializeField] private DevTools devTools;
        [SerializeField] private PlayerSFXMaster sfxHandler;
        [SerializeField] private CameraFollowObject followObject;
        [SerializeField] private PlayerSaveHandler saveHandler;
        [SerializeField] private ReactiveBurstParticleController burstParticlesController;

        [Header("Fields - Physics")]
        [SerializeField] private float gravity;
        [SerializeField] private float maxJumpVelocity;
        [SerializeField] private float minJumpVelocity;
        [SerializeField] private bool fastFalling;
        [SerializeField] private int moveDirectionX;
        [SerializeField] private int manualMoveX;
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float groundPredictionAmount;
        [SerializeField] private float standCheckDist;

        [Header("Fields - Movement")]
        [SerializeField] private float xVelSmoothing;
        [SerializeField] private float lastPressedMoveTime;

        [Header("Fields - Jump")]
        [SerializeField] private bool isJumpCut;
        [SerializeField] private bool isJumping;
        [SerializeField] private float lastOnGroundTime;
        [SerializeField] private float lastPressedJumpTime;

        [Header("Fields - Wall Slide")]
        [SerializeField] private int wallDirX;
        [SerializeField] private float timeToWallUnstick;

        [Header("Fields - Fast Slide")]
        [SerializeField] private float currentSlideScalar;

        [Header("Fields - Checks")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool isWallSliding;
        [SerializeField] private bool isBouncing;
        [SerializeField] private bool isSlopeBouncing;
        [SerializeField] private bool isWallJumping;
        [SerializeField] private bool isAgainstWall;
        [SerializeField] private bool isThrowing;
        [SerializeField] private bool isThrowingAgain;
        [SerializeField] private bool hasFireflies;
        [SerializeField] private bool tryFastSlide;
        [SerializeField] private bool isFastSliding;
        [SerializeField] private bool isCrawling;
        [SerializeField] private bool isOnSlope;

        private Coroutine disableTimer;
        private Blackboard playerBlackboard;
        private BlackboardKey isCrawlingKey;

        private float fallSpeedDampingChangeThreshold;

        private StateMachine stateMachine;

        public bool IsGrounded { get { return isGrounded; } }
        public bool IsFastSliding { get { return isFastSliding; } set { isFastSliding = value; } }
        public bool IsCrawling { get { return isCrawling; } set { isCrawling = value; } }
        public Vector2 Velocity { get { return velocity; } }
        public (Vector2 Offset, Vector2 Size) CrawlingCollider { get; private set; }

        private void Awake()
        {
            // Get components
            animator = GetComponentInChildren<Animator>();
            collisionHandler = GetComponent<DynamicCollisionHandler>();
            devTools = GetComponentInChildren<DevTools>();
            sfxHandler = GetComponentInChildren<PlayerSFXMaster>();
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            followObject = GetComponentInChildren<CameraFollowObject>();
            saveHandler = GetComponent<PlayerSaveHandler>();

            // Declare states
            stateMachine = new StateMachine();
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator, sfxHandler);
            JumpState jumpState = new JumpState(this, animator, sfxHandler);
            SlideState slideState = new SlideState(this, animator, sfxHandler);
            CrawlIdleState crawlIdleState = new CrawlIdleState(this, animator, boxCollider, collisionHandler);
            CrawlLocomotionState crawlLocomotionState = new CrawlLocomotionState(this, animator, sfxHandler, boxCollider, collisionHandler);
            WallState wallState = new WallState(this, animator, sfxHandler);
            FallState fallState = new FallState(this, animator, sfxHandler);
            LandState landState = new LandState(this, animator, sfxHandler);
            BounceState bounceState = new BounceState(this, animator);
            WallJumpState wallJumpState = new WallJumpState(this, animator, sfxHandler);
            ThrowIdleState throwIdleState = new ThrowIdleState(this, animator, sfxHandler);
            ThrowLocomotionState throwLocomotionState = new ThrowLocomotionState(this, animator, sfxHandler);
            DevState devState = new DevState(this, devTools, animator);

            // Define strict transitions
            At(idleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            At(idleState, jumpState, new FuncPredicate(() => isJumping));
            At(idleState, throwIdleState, new FuncPredicate(() => isThrowing));
            At(idleState, slideState, new FuncPredicate(() => isFastSliding));
            At(idleState, crawlIdleState, new FuncPredicate(() => isCrawling && moveDirectionX == 0));
            At(idleState, crawlLocomotionState, new FuncPredicate(() => isCrawling && moveDirectionX != 0));

            At(locomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
            At(locomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(locomotionState, throwLocomotionState, new FuncPredicate(() => isThrowing));
            At(locomotionState, slideState, new FuncPredicate(() => isFastSliding));
            At(locomotionState, crawlIdleState, new FuncPredicate(() => isCrawling && moveDirectionX == 0));
            At(locomotionState, crawlLocomotionState, new FuncPredicate(() => isCrawling && moveDirectionX != 0));

            At(jumpState, locomotionState, new FuncPredicate(() => isGrounded && !isJumping));
            At(jumpState, wallState, new FuncPredicate(() => isWallSliding));

            At(slideState, idleState, new FuncPredicate(() => !isFastSliding && moveDirectionX == 0));
            At(slideState, locomotionState, new FuncPredicate(() => !isFastSliding && moveDirectionX != 0));
            At(slideState, jumpState, new FuncPredicate(() => isJumping));

            At(crawlIdleState, idleState, new FuncPredicate(() => !isCrawling && moveDirectionX == 0));
            At(crawlIdleState, locomotionState, new FuncPredicate(() => !isCrawling && moveDirectionX != 0));
            At(crawlIdleState, crawlLocomotionState, new FuncPredicate(() => isCrawling && moveDirectionX != 0));

            At(crawlLocomotionState, idleState, new FuncPredicate(() => !isCrawling && moveDirectionX == 0));
            At(crawlLocomotionState, locomotionState, new FuncPredicate(() => !isCrawling && moveDirectionX != 0));
            At(crawlLocomotionState, crawlIdleState, new FuncPredicate(() => isCrawling && moveDirectionX == 0));

            At(wallState, idleState, new FuncPredicate(() => isGrounded && moveDirectionX == 0));
            At(wallState, jumpState, new FuncPredicate(() => isJumping));
            At(wallState, wallJumpState, new FuncPredicate(() => isWallJumping));

            At(fallState, wallState, new FuncPredicate(() => isWallSliding));
            At(fallState, landState, new FuncPredicate(() => isGrounded));

            At(landState, idleState, new FuncPredicate(() => isGrounded && moveDirectionX == 0));
            At(landState, locomotionState, new FuncPredicate(() => isGrounded && moveDirectionX != 0));

            At(bounceState, idleState, new FuncPredicate(() => isGrounded && moveDirectionX == 0));
            At(bounceState, locomotionState, new FuncPredicate(() => isGrounded && moveDirectionX != 0));
            At(bounceState, wallState, new FuncPredicate(() => isWallSliding));

            At(wallJumpState, wallState, new FuncPredicate(() => isWallSliding));
            At(wallJumpState, idleState, new FuncPredicate(() => isGrounded && moveDirectionX == 0));
            At(wallJumpState, locomotionState, new FuncPredicate(() => isGrounded && moveDirectionX != 0));

            At(throwIdleState, idleState, new FuncPredicate(() => !isThrowing && moveDirectionX == 0));
            At(throwIdleState, locomotionState, new FuncPredicate(() => moveDirectionX != 0));
            At(throwIdleState, jumpState, new FuncPredicate(() => isJumping));
            At(throwIdleState, throwIdleState, new FuncPredicate(() => isThrowingAgain));
            At(throwIdleState, slideState, new FuncPredicate(() => isFastSliding));

            At(throwLocomotionState, idleState, new FuncPredicate(() => moveDirectionX == 0));
            At(throwLocomotionState, locomotionState, new FuncPredicate(() => !isThrowing && moveDirectionX != 0));
            At(throwLocomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(throwLocomotionState, throwLocomotionState, new FuncPredicate(() => isThrowingAgain));
            At(throwLocomotionState, slideState, new FuncPredicate(() => isFastSliding));

            At(devState, idleState, new FuncPredicate(() => !devTools.Active && moveDirectionX == 0));
            At(devState, locomotionState, new FuncPredicate(() => !devTools.Active && moveDirectionX != 0));

            // Define any transitions
            Any(devState, new FuncPredicate(() => devTools.Active));
            Any(fallState, new FuncPredicate(() => !devTools.Active && !isGrounded && !isWallSliding && velocity.y < 0f));
            Any(bounceState, new FuncPredicate(() => !devTools.Active && isBouncing));

            // Set an initial state
            stateMachine.SetState(idleState);

            // Set crawling values
            Vector2 crawlOffset = new Vector2(-0.05183601f, -0.4001744f);
            Vector2 crawlSize = new Vector2(0.8475914f, 0.5710797f);

            // Set default values
            Vector2 defaultOffset = new Vector2(-0.009529829f, -0.1905082f);
            Vector2 defaultSize = new Vector2(0.5014615f, 0.9904121f);

            float crawlingTop = crawlOffset.y + (crawlSize.y / 2.0f);
            float standingTop = defaultOffset.y + (defaultSize.y / 2.0f);

            // Calculate the y-distance between the tops of the colliders
            standCheckDist = standingTop - crawlingTop;
        }

        private void Start()
        {
            // Send out the player transform and controller
            onSendPlayerTransform.Raise(this, transform);
            onSendPlayerController.Raise(this, this);

            // Define gravity
            gravity = -(2 * data.maxJumpHeight) / Mathf.Pow(data.timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity * data.timeToJumpApex);
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * data.minJumpHeight);

            // Set the fall speed change threshold
            fallSpeedDampingChangeThreshold = CameraManager.Instance.FallSpeedDampingChangeThreshold;

            // Register blackboard
            playerBlackboard = BlackboardController.Instance.GetBlackboard("Player");
            isCrawlingKey = playerBlackboard.GetOrRegisterKey("IsCrawling");

            // Set values for the blackboard
            ChangeBlackboardValue(isCrawlingKey, false);
        }

        private void OnEnable()
        {
            input.Move += SetTurnTimer;
            input.Jump += OnJump;
            input.FastFall += OnFastFall;
            input.FastSlide += OnFastSlide;
            input.Crawl += OnCrawl;
        }

        private void OnDisable()
        {
            input.Move -= SetTurnTimer;
            input.Jump -= OnJump;
            input.FastFall -= OnFastFall;
            input.FastSlide -= OnFastSlide;
            input.Crawl -= OnCrawl;
        }

        private void Update()
        {
            // Update movement direction
            if (input.AllowControl)
                moveDirectionX = input.NormMoveX;
            else
                moveDirectionX = manualMoveX;

            // Check crawling so that if the player is not holding crawl, but they can stand,
            // they automatically stand
            if(!input.HoldingCrawl && isCrawling && collisionHandler.collisions.CanStand)
            {
                isCrawling = false;
                ChangeBlackboardValue(isCrawlingKey, isCrawling);
            }

            // Update timers
            UpdateTimers();

            // Update player position
            onPositionChange.Raise(this, (Vector2)transform.position);

            // Update the state machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Update the state machine
            stateMachine.FixedUpdate();

            // Update sprite
            CheckDirectionToFace();
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Add a transition from any State to another one given a certain condition
        /// </summary>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the transition</param>
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        /// <summary>
        /// Calculate the initial velocity of movement before handling
        /// </summary>
        public void CalculateVelocity()
        {
            // If fast sliding, set the move direction
            if(isFastSliding)
            {
                moveDirectionX = collisionHandler.collisions.SlopeDescentDirection;
            }

            // Check if the player is grounded
            if (collisionHandler.collisions.Below)
            {
                // Cancel fast slides if not descending a slope
                if (!collisionHandler.collisions.DescendingSlope)
                    isFastSliding = false;

                // Set grounded to true
                isGrounded = true;

                isWallJumping = false;

                // Reset bounce
                if(isBouncing && velocity.y <= 0f)
                {
                    // Reset bounce data
                    ResetBounce();
                }

                // Check to reset bounce count or other variables
                // that don't rely on the isBouncing variable
                if(velocity.y <= 0f)
                {
                    // Reset bounce variables
                    // Calls to:
                    //  - MushroomBounce.ResetBounce()
                    onResetBounce.Raise(this, null);
                }

                // Set coyote time
                lastOnGroundTime = data.coyoteTime;

                if(lastPressedJumpTime > 0f)
                {
                    // Set jumping to true
                    isJumping = true;

                    // Reset last pressed jump time
                    lastPressedJumpTime = 0f;

                    // Execute the jump
                    HandleJump();
                }

                // Allow the player to peek
                onSetCanPeek.Raise(this, true);
            }
            else isGrounded = false;

            // Cancel jumps and jump cuts once grounded
            if(collisionHandler.collisions.Below && velocity.y <= 0f)
            {
                if (isJumping)
                    isJumping = false;

                if (isJumpCut)
                    isJumpCut = false;
            }

            // Check if wall sliding
            if ((collisionHandler.collisions.Left || collisionHandler.collisions.Right) &&
                !collisionHandler.collisions.Below && velocity.y < 0 &&
                collisionHandler.collisions.Layer == CollisionLayer.MushroomWall)
            {
                // Cancel jumps and jump cuts once on a wall
                if (isJumping)
                    isJumping = false;

                if (isJumpCut)
                    isJumpCut = false;

                // Set wall sliding to true
                isWallSliding = true;
            } else
            {
                isWallSliding = false;
            }

            // Check if against a wall
            if((collisionHandler.collisions.Left || collisionHandler.collisions.Right) &&
                !isGrounded &&
                collisionHandler.collisions.Layer == CollisionLayer.MushroomWall)
            {
                isAgainstWall = true;
            } else
            {
                isAgainstWall = false;
            }

            // Get the target speed
            if (!isWallJumping && !isSlopeBouncing)
            {
                // Get the target speed (depending on if crawling or not)
                float targetSpeed = (!isCrawling) 
                    ? moveDirectionX * data.movementSpeed 
                    : moveDirectionX * data.crawlSpeed;

                // Smooth the target speed
                velocity.x = Mathf.SmoothDamp(
                    velocity.x,
                    targetSpeed,
                    ref xVelSmoothing,
                    (collisionHandler.collisions.Below) ? data.accelerationTimeGround : data.accelerationTimeAir
                );
            }

            // Handle movement when slope bouncing
            if(isSlopeBouncing)
            {
                // Get the input speed
                float inputSpeed = moveDirectionX * data.movementSpeed;

                // Adjust the speed depending on if moving in the same direction as the bounce
                float adjustedSpeed = (Mathf.Sign(inputSpeed) == Mathf.Sign(velocity.x)) 
                    ? inputSpeed * data.shroomSlopeInputScalar // Apply the scalar
                    : inputSpeed; // Use the normal input speed

                // Smooth the target speed
                velocity.x = Mathf.SmoothDamp(
                    velocity.x,
                    adjustedSpeed + velocity.x,
                    ref xVelSmoothing,
                    (collisionHandler.collisions.Below) ? data.accelerationTimeGround : data.accelerationTimeAir
                );
            }

            // Apply gravity if not wall sliding - wall sliding handles gravity
            // on it's own
            if (!isWallSliding)
                HandleFalling();
        }

        /// <summary>
        /// Handle player falling
        /// </summary>
        public void HandleFalling()
        {
            // Check if grounded
            if(isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                return;
            }

            // Check if fast falling
            if(fastFalling)
            {
                // Learn the fast fall control
                LearnControl("Fast Fall");

                // Increase the gravity by the scalar
                velocity.y += gravity * data.fastFallScalar * Time.deltaTime;

                // Clamp to fast fall speed
                if(velocity.y < -data.maxFastFallSpeed)
                {
                    velocity.y = -data.maxFastFallSpeed;
                }
            } else
            {
                // Use normal gravity
                velocity.y += gravity * Time.deltaTime;

                // If not, clamp to normal fall speed
                if (velocity.y < -data.maxFallSpeed)
                {
                    velocity.y = -data.maxFallSpeed;
                }
            }

            sfxHandler.SetFallSpeedRTPC(Mathf.Abs(velocity.y));

            // Show more underneath the player if they are falling
            if(velocity.y <= fallSpeedDampingChangeThreshold &&
                !CameraManager.Instance.IsLerpingFallOffset && !CameraManager.Instance.LerpedFromPlayerFalling
                && !collisionHandler.collisions.CameraWithinGroundDistance)
            {
                CameraManager.Instance.LerpFallOffset(true);
            }

            // Reset the camera when predicting ground to avoid bounciness with the camera re-adjusting
            if(velocity.y <= fallSpeedDampingChangeThreshold && CameraManager.Instance.LerpedFromPlayerFalling 
                && collisionHandler.collisions.CameraWithinGroundDistance)
            {
                CameraManager.Instance.LerpedFromPlayerFalling = false;
                CameraManager.Instance.LerpFallOffset(false);
            }
        }

        /// <summary>
        /// Handle the player movement
        /// </summary>
        public void HandleMovement()
        {
            // Move
            Move(velocity * Time.deltaTime);

            // If there's a collision above or below, set the y-velocity to 0 to stop
            // from accumulating gravity
            if (collisionHandler.collisions.Above || collisionHandler.collisions.Below && !isJumping)
            {
                // Check if sliding down slope
                if (collisionHandler.collisions.SlidingDownMaxSlope)
                {
                    // If so, set the y-velocity to the slope normal multiplied by gravity
                    velocity.y += collisionHandler.collisions.SlopeNormal.y * -gravity * Time.deltaTime;
                }
                else
                {
                    // Otherwise, set y-velocity to 0
                    velocity.y = 0f;
                }
            }

            // Check if the camera should be lerped at a base level
            if ((collisionHandler.collisions.Layer == CollisionLayer.Slope && !CameraManager.Instance.IsLerpingSlideOffset && !CameraManager.Instance.LerpedFromPlayerSliding
                && collisionHandler.collisions.LastSlopeVerticalDirection != 0) ||
                (collisionHandler.collisions.Layer == CollisionLayer.Slope && 
                collisionHandler.collisions.PrevLastSlopeVerticalDirection != collisionHandler.collisions.LastSlopeVerticalDirection))
            {
                bool changedDirections = 
                    (collisionHandler.collisions.LastSlopeVerticalDirection != 0 && collisionHandler.collisions.PrevLastSlopeVerticalDirection != 0)
                    ? collisionHandler.collisions.PrevLastSlopeVerticalDirection != collisionHandler.collisions.LastSlopeVerticalDirection
                    : false;

                // Get the camera direction
                Vector2 cameraDirection = new Vector2(
                    (int)Mathf.Sign(collisionHandler.collisions.SlopeNormal.x),
                    collisionHandler.collisions.LastSlopeVerticalDirection
                );

                // Lerp the slide
                CameraManager.Instance.LerpSlopeOffset(followObject, cameraDirection, true, changedDirections);
            }

            // If grounded, or moving upwards, reset the camera
            if (velocity.y >= 0f && CameraManager.Instance.LerpedFromPlayerFalling && collisionHandler.collisions.Layer != CollisionLayer.Slope)
            {
                CameraManager.Instance.LerpedFromPlayerFalling = false;
                CameraManager.Instance.LerpFallOffset(false);
            }

            // If moving upwards or lerping from player sliding and not descending slopes, reset the camera
            if(velocity.y >= 0f && CameraManager.Instance.LerpedFromPlayerSliding && collisionHandler.collisions.Layer != CollisionLayer.Slope)
            {
                CameraManager.Instance.LerpedFromPlayerSliding = false;
                CameraManager.Instance.LerpSlopeOffset(followObject, Vector2.zero, false);
            }
        }

        /// <summary>
        /// Move the player
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="standingOnPlatform"></param>
        public void Move(Vector2 velocity, bool standingOnPlatform = false)
        {
            // Update raycasts
            collisionHandler.UpdateRaycastOrigins();

            // Reset collisions
            collisionHandler.collisions.ResetInfo();

            // Set the old velocity
            collisionHandler.collisions.PrevVelocity = velocity;

            // Handle collisions if necessary
            if (velocity.y < 0f)
                collisionHandler.DescendSlope(ref velocity, tryFastSlide, this, currentSlideScalar);

            // Set the facing direction
            if (velocity.x != 0f)
            {
                collisionHandler.collisions.FacingDirection = (int)Mathf.Sign(velocity.x);
            }

            // Handle horizontal collisions
            collisionHandler.HorizontalCollisions(ref velocity);

            // Handle vertical collisions
            if (velocity.y != 0f)
                collisionHandler.VerticalCollisions(ref velocity);

            // Predict ground
            collisionHandler.PredictGround(velocity, groundPredictionAmount);

            // Check if the player can stand
            if (isCrawling)
                collisionHandler.CheckCanStand(velocity, standCheckDist);

            // Move
            transform.Translate(velocity);

            if (standingOnPlatform)
            {
                collisionHandler.collisions.Below = true;
            }
        }

        public void CalculateSliding()
        {
            // Exit case - not fast sliding
            if (!isFastSliding) return;

            // Increase the slide speed
            currentSlideScalar += data.slideAcceleration * Time.deltaTime;

            // Clamp the slide scalar
            currentSlideScalar = Mathf.Clamp(currentSlideScalar, 1f, data.maxFastSlideScalar);
        }

        #region JUMP HANDLING
        /// <summary>
        /// Handle the upward movement of the jump
        /// </summary>
        private void HandleJump()
        {
            // Check if grounded
            if (collisionHandler.collisions.Below || lastOnGroundTime > 0 && !isJumpCut)
            {
                // Check if sliding down a slope
                if (collisionHandler.collisions.SlidingDownMaxSlope)
                {
                    // Check if we are not jumping against a max slope
                    if (input.NormMoveX != -Mathf.Sign(collisionHandler.collisions.SlopeNormal.x))
                    {
                        velocity.y = maxJumpVelocity * collisionHandler.collisions.SlopeNormal.y;
                        velocity.x = maxJumpVelocity * collisionHandler.collisions.SlopeNormal.x;
                    }
                }
                else
                {
                    // Otherwise, jump like normal
                    velocity.y = maxJumpVelocity;
                }
            }

            if(isJumpCut)
            {
                // Cut velocity
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }
        }

        /// <summary>
        /// Begin the jump
        /// </summary>
        public void StartJump()
        {
            // Set jumping to true
            isJumping = true;

            // Set jump cut to false
            isJumpCut = false;

            // Handle a normal jump
            HandleJump();
        }
        #endregion

        #region BOUNCE HANDLING
        /// <summary>
        /// Reset the player's bounce variables
        /// </summary>
        public void ResetBounce()
        {
            // Set bouncing to false and update the events
            isBouncing = false;
            isSlopeBouncing = false;
        }

        /// <summary>
        /// Bounce the player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Bounce(Component sender, object data)
        {
            // Check if the data is the correct type
            if (data is not MushroomBounce.BounceData) return;

            // Cast data
            MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;

            // Cancel jumping and jump cuts
            if (isJumping)
                isJumping = false;
            if (isJumpCut)
                isJumpCut = false;

            // Reset crawling
            if (isCrawling)
            {
                isCrawling = false;
                ChangeBlackboardValue(isCrawlingKey, isCrawling);
            }

            lastPressedJumpTime = 0;

            // Set bouncing to true
            isBouncing = true;

            isSlopeBouncing = bounceData.Rotated;

            // Calculate bounce force
            Vector2 bounceVec = bounceData.BounceVector;

            int clampedBounceCount = Mathf.Clamp(bounceData.BounceCount, 1, 3);

            switch (clampedBounceCount)
            {
                case 2:
                    bounceVec *= this.data.secondBounceMult;
                    break;

                case 3:
                    // Learn the chain bounce
                    onLearnChainBounce.Raise(this, null);

                    bounceVec *= this.data.thirdBounceMult;
                    break;

                default:
                    bounceVec *= 1f;
                    break;
            }

            // Apply bounce force
            velocity.x += bounceVec.x;
            velocity.y = bounceVec.y;

            // Play the bounce sound
            sfxHandler.Bounce(Mathf.Clamp(bounceData.BounceCount, 1, 4));
        }
        #endregion

        #region WALL JUMP HANDLING
        public void StartWallJump()
        {
            // Set wall sliding to false
            isWallSliding = false;

            // Set wall jumping to true
            isWallJumping = true;

            // Wall jump
            // Calls to:
            //  - MushroomWallJump.GetSpawnData
            onWallJumpInput.Raise(this, collisionHandler.collisions.HorizontalCollisionRay);
        }

        /// <summary>
        /// Set the player's wall jumping state
        /// </summary>
        /// <param name="isWallJumping"></param>
        public void SetWallJumping(bool isWallJumping) => this.isWallJumping = isWallJumping;

        /// <summary>
        /// Start a wall jump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void StartWallJump(Component sender, object data)
        {
            // Verify that the data being sent is correct
            if (data is not Vector2) return;

            // Cast data
            Vector2 wallJumpVector = (Vector2)data;

            // Set data
            velocity.x = wallJumpVector.x;
            velocity.y = wallJumpVector.y;
        }

        /// <summary>
        /// Handle the wall sliding of the player
        /// </summary>
        public void HandleWallSliding()
        {
            // Get the direction of the wall
            wallDirX = (collisionHandler.collisions.Left) ? -1 : 1;

            // Check if wall sliding
            if (isWallSliding)
            {
                // Check if fast falling
                if (fastFalling)
                {
                    // Increase the gravity by the scalar
                    velocity.y += gravity * data.fastWallSlideScalar * Time.deltaTime;

                    // Clamp to the max fast wall slide speed
                    if (velocity.y < -data.maxFastWallSlideSpeed)
                    {
                        velocity.y = -data.maxFastWallSlideSpeed;
                    }
                }
                else
                {
                    // Use normal gravity
                    velocity.y += gravity * Time.deltaTime;

                    // Clamp to the max wall slide speed
                    if (velocity.y < -data.maxWallSlideSpeed)
                    {
                        velocity.y = -data.maxWallSlideSpeed;
                    }
                }

                // Check if we should be sticking to the wall
                if (timeToWallUnstick > 0f)
                {
                    // Set x-velocity and smoothing to 0
                    xVelSmoothing = 0f;
                    velocity.x = 0f;

                    // Check if we need to stick to the wall
                    if (input.NormMoveX != wallDirX && input.NormMoveX != 0f)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        // Reset the wall stick time
                        timeToWallUnstick = data.wallStickTime;
                    }
                }
            }
        }
        #endregion

        #region THROW HANDLING
        /// <summary>
        /// Start the player throw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void StartThrow(Component sender, object data)
        {
            // Don't allow for throws in the air, will cause a throw animation when landing,
            // long after the throw was done
            if (!isGrounded) return;

            // Return if crawling
            if (isCrawling) return;

            // Check if throwing
            if (!isThrowing)
                // If not, set the player to throwing
                isThrowing = true;
            else
                isThrowingAgain = true;
        }

        public void SetThrowingAgain(bool isThrowingAgain) => this.isThrowingAgain = isThrowingAgain;

        /// <summary>
        /// Set the player throw
        /// </summary>
        /// <param name="isThrowing"></param>
        public void SetThrow(bool isThrowing) => this.isThrowing = isThrowing;
        #endregion

        #region TRANSITION HANDLING
        public void PreparePlayerPosition(Component sender, object data)
        {
            // Verify the correct data is sent
            if (data is not (Vector2, int)) return;

            // Cast the data
            (Vector2 pos, int dir) = ((Vector2 pos, int dir))data;

            // Set the player transform
            transform.position = pos;

            // Set the player direction
            manualMoveX = dir;

            // Force a save
            saveHandler.LevelPositionUpdate();
        }

        /// <summary>
        /// Function for handling the beginning of a level transition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void BeginPlayerTransition(Component sender, object data)
        {
            // Verify that the correct data is sent
            if (data is not int) return;

            // Cast the data
            int manualMove = (int)data;

            // Disable control
            input.AllowControl = false;

            // Set movement direction
            moveDirectionX = 0;
            manualMoveX = manualMove;
        }

        /// <summary>
        /// Function for handling the end of a level transition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void EndPlayerTransition(Component sender, object data)
        {
            // Enable cotrol
            input.AllowControl = true;
        }
        #endregion

        #region INPUT
        /// <summary>
        /// Handle jump input
        /// </summary>
        /// <param name="started"></param>
        public void OnJump(bool started)
        {
            // Check if the context was started
            if (started)
            {
                // Check for wall jumps first
                if (isAgainstWall)
                {
                    // Start the wall jump
                    StartWallJump();
                }

                // If not bouncing, wall jumping, or crawling, do a normal jump
                if (!isBouncing && !isWallJumping && !isCrawling)
                {
                    // Check if jumping already
                    if (!isJumping)
                    {
                        // Start the jump if not
                        StartJump();
                    }
                    else
                    {
                        // Jump cut if so
                        isJumpCut = false;

                        // Set the last pressed jump time
                        lastPressedJumpTime = data.jumpBufferTime;
                    }
                }
            }

            // Check if the context was canceled and that the input still applies
            if (!started && isJumping)
            {
                // Set jump cutting
                isJumpCut = true;

                // Handle the jump
                HandleJump();
            }
        }

        /// <summary>
        /// Handle fast fall input
        /// </summary>
        /// <param name="started"></param>
        public void OnFastFall(bool started)
        {
            if (started)
            {
                fastFalling = true;
            }

            if (!started)
            {
                fastFalling = false;
            }
        }

        /// <summary>
        /// Handle fast slide input
        /// </summary>
        /// <param name="started"></param>
        public void OnFastSlide(bool started)
        {
            if (started)
            {
                tryFastSlide = true;
            }

            if (!started)
            {
                EndSlide();
            }
        }

        /// <summary>
        /// Handle crawl input
        /// </summary>
        /// <param name="started"></param>
        public void OnCrawl(bool started)
        {
            if (started && isGrounded)
            {
                isCrawling = true;
                ChangeBlackboardValue(isCrawlingKey, isCrawling);
            }

            if (!started)
            {
                // If the player can't stand, return
                if (!collisionHandler.collisions.CanStand) return;

                // Otherwise, set crawling to false
                isCrawling = false;
                ChangeBlackboardValue(isCrawlingKey, isCrawling);
            }
        }
        #endregion

        #region HELPERS
        /// <summary>
        /// Set the timer to turn and activate LookAhead
        /// </summary>
        /// <param name="turnTimer"></param>
        private void SetTurnTimer(Vector2 turnTimer) => lastPressedMoveTime = data.movementTurnTime;

        /// <summary>
        /// Check the direction for the player to face
        /// </summary>
        private void CheckDirectionToFace()
        {
            Vector3 scale = transform.localScale;
            scale.x = collisionHandler.collisions.FacingDirection;
            transform.localScale = scale;

            // Turn the camera
            // Calls to:
            //  - CameraFollowObject.CallTurn();
            if (lastPressedMoveTime <= 0f)
                onPlayerTurn.Raise(this, collisionHandler.collisions.FacingDirection);
        }

        /// <summary>
        /// Update movement timers
        /// </summary>
        public void UpdateTimers()
        {
            if (lastOnGroundTime > 0f)
                lastOnGroundTime -= Time.deltaTime;

            if (lastPressedJumpTime > 0f)
                lastPressedJumpTime -= Time.deltaTime;

            if (lastPressedMoveTime > 0f)
                lastPressedMoveTime -= Time.deltaTime;
        }

        public void EndSlide()
        {
            // Reset slide variables
            tryFastSlide = false;
            isFastSliding = false;
            currentSlideScalar = 1f;
        }

        /// <summary>
        /// Wrapper function to learn a control
        /// </summary>
        /// <param name="controlName">The name of the control</param>
        public void LearnControl(string controlName) => onLearnControl.Raise(this, controlName);

        /// <summary>
        /// Set whether or not the player can peek or not
        /// </summary>
        /// <param name="canPeek">Whether the player can peek or not</param>
        public void SetCanPeek(bool canPeek)
        {
            // Calls to:
            //  - CameraPeek.SetCanPeek();
            onSetCanPeek.Raise(this, canPeek);
        }

        /// <summary>
        /// Play a particle effect
        /// </summary>
        /// <param name="particleType">An int representing the particle type (0 for Jump, 1 for Land)</param>
        public void PlayParticles(int particleType)
        {
            if (burstParticlesController == null) return;

            // Handle particle types
            switch (particleType)
            {
                case 0:
                    //particlesController.HandleJumpParticles();
                    burstParticlesController.HandleJumpParticles();
                    break;

                case 1:
                    //particlesController.HandleLandParticles();
                    burstParticlesController.HandleLandParticles();
                    break;
            }
        }

        /// <summary>
        /// Change a Blackboard value
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void ChangeBlackboardValue(BlackboardKey key, bool value)
        {
            if (playerBlackboard.TryGetValue(key, out bool blackboardValue))
                playerBlackboard.SetValue(key, value);
        }

        /// <summary>
        /// Disable the player for a certain amount of time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void DisablePlayerTimed(Component sender, object data)
        {
            // Verify that the correct data is sent
            if (data is not float) return;

            // Cast the data
            float disableDuration = (float)data;

            // Disable input
            input.Disable();

            // Zero out velocity to prevent movement
            velocity = Vector2.zero;

            // If disable timer is not null, nullify it
            if (disableTimer != null)
                StopCoroutine(disableTimer);

            // Start the disabled timer
            disableTimer = StartCoroutine(DisableTimer(disableDuration, sender));
        }

        /// <summary>
        /// Coroutine to wait for a specific duration before re-enabling input
        /// </summary>
        /// <param name="duration">The duration to wait before re-enabling input</param>
        /// <returns></returns>
        public IEnumerator DisableTimer(float duration, Component sender)
        {
            // Start a timer
            float elapsedTime = 0f;

            // Wait for the duration
            while(elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Re-enable input
            input.Enable();

            // If the sender is a mushroom pickup, stop the rumble sound
            if (sender is MushroomPickup)
                ((MushroomPickup)sender).StopRumble();
        }

        /// <summary>
        /// Set player input
        /// </summary>
        /// <param name="enabled">True if player input should be enabled, false if otherwise</param>
        public void SetPlayerInput(bool enabled)
        {
            // If enabled, enable input
            if (enabled)
                input.Enable();
            // If not, disable input
            else
                input.Disable();
        }

        /// <summary>
        /// Try to force a state change
        /// </summary>
        /// <param name="stateNum"></param>
        public void TrySetState(int stateNum)
        {
            switch(stateNum)
            {
                // None
                case 0:
                    tryFastSlide = false;
                    isWallSliding = false;
                    isBouncing = false;
                    isWallJumping = false;
                    isThrowing = false; ;
                    isThrowingAgain = false;
                    isFastSliding = false;
                    isCrawling = false;
                    break;

                // Idle
                case 1:
                    velocity = Vector2.zero;
                    break;

                // Slide
                case 2:
                    tryFastSlide = true;
                    break;
            }
        }
        #endregion
    }
}