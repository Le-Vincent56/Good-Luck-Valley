using GoodLuckValley.Events;
using GoodLuckValley.UI;
using GoodLuckValley.Player.StateMachine.States;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Player.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoodLuckValley.Persistence;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerController : MonoBehaviour
    {
        #region EVENTS
        [SerializeField] private GameEvent onRequestPowerUnlocks;
        #endregion

        #region REFERENCES
        [SerializeField] private PlayerData playerData;
        [SerializeField] private PlayerSaveHandler playerSaveHandler;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheck;
        #endregion

        #region FIELDS
        [SerializeField] private PhysicsMaterial2D noFriction;
        [SerializeField] private string previousState;
        [SerializeField] private string currentState;
        private float wallCheckDist = 0.3f;

        private (float x, float y) colliderSize;
        private (Vector2 bottomLeft, Vector2 bottomRight, Vector2 middleLeft, Vector2 middleRight) raycastOrigins;
        private Vector2 frontRaycast;
        private Vector2 backRaycast;
        private SlopeDirection slopeDirection;

        public bool isLocked = false;
        private bool isFacingRight = true;
        private bool isBouncing = false;
        private bool isWallJumping = false;
        private int lastMovementDirection;
        #endregion

        #region PROPERTIES
        public Rigidbody2D RB { get; private set; }
        public BoxCollider2D PlayerCollider { get; private set; }
        public Animator Anim { get; private set; }
        public PowerController Power { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
        public PlayerFastFallState FastFallState { get; private set; }
        public PlayerInAirState InAirState { get; private set; }
        public PlayerLandState LandState { get; private set; }
        public PlayerBounceState BounceState { get; private set; }
        public PlayerWallSlideState WallSlideState { get; private set; }
        public PlayerFastWallSlideState FastWallSlideState { get; private set; }
        public PlayerWallJumpState WallJumpState { get; private set; }
        public PlayerSlopeState SlopeState { get; private set; }
        public bool IsFacingRight { get { return isFacingRight; } set {  isFacingRight = value; } }
        #endregion

        private void Awake()
        {
            // Create a new state machine
            StateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
            MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
            JumpState = new PlayerJumpState(this, StateMachine, playerData, "jump");
            FallState = new PlayerFallState(this, StateMachine, playerData, "fall");
            FastFallState = new PlayerFastFallState(this, StateMachine, playerData, "fastFall");
            InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
            LandState = new PlayerLandState(this, StateMachine, playerData, "land");
            BounceState = new PlayerBounceState(this, StateMachine, playerData, "bounce");
            WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wall");
            FastWallSlideState = new PlayerFastWallSlideState(this, StateMachine, playerData, "fastWall");
            WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "wallJump");
            SlopeState = new PlayerSlopeState(this, StateMachine, playerData, "slope");

            // Get components
            RB = GetComponent<Rigidbody2D>();
            PlayerCollider = GetComponent<BoxCollider2D>();
            Anim = GetComponent<Animator>();
            Power = GetComponentInChildren<PowerController>();
            InputHandler = GetComponent<PlayerInputHandler>();
        }
        private void Start()
        {
            // Set collider size
            colliderSize.x = PlayerCollider.size.x / 2f;
            colliderSize.y = PlayerCollider.size.y / 2f;

            // Set raycast origins
            UpdateRaycasts();

            // Enter the idle state if no other state has been loaded
            if(StateMachine.CurrentState == null) StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // Run the logic updates for the current state
            StateMachine.CurrentState.LogicUpdate();

            // Check and set states
            if(StateMachine.PreviousState != null)
                previousState = StateMachine.PreviousState.ToString().Substring(48);
            currentState = StateMachine.CurrentState.ToString().Substring(48);

            // Update raycasts
            UpdateRaycasts();

            // Set the last input handler
            if(InputHandler.NormInputX != 0f) lastMovementDirection = InputHandler.NormInputX;

            // Check to reset jump input
            if (InputHandler.JumpInput && 
                CheckState() != IdleState && CheckState() != MoveState && CheckState() != LandState)
            {
                InputHandler.UseJumpInput();
            }
        }

        private void FixedUpdate()
        {
            // Run physics updates for the current state
            StateMachine.CurrentState.PhysicsUpdate();
        }

        #region CHECK FUNCTIONS
        /// <summary>
        /// Check if the Player is grounded
        /// </summary>
        /// <returns>True if the Player is grounded, false if not</returns>
        public bool CheckIfGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, playerData.groundRadius, playerData.groundLayer) ||
                Physics2D.OverlapCircle(groundCheck.position, playerData.groundRadius, playerData.wallLayer);
        }

        /// <summary>
        /// Check if the Player is on top of a Mushroom Wall
        /// </summary>
        /// <returns>True if the Player is on top of a Mushroom Wall, false if not</returns>
        public bool CheckIfOnTopOfMushWall()
        {
            return Physics2D.OverlapCircle(groundCheck.position, playerData.groundRadius, playerData.mushroomWallLayer);
        }

        /// <summary>
        /// Use Raycasts to check if the Player is grounded
        /// </summary>
        /// <returns>True if the Player is grounded, false if not</returns>
        public bool CheckIfGroundedLine()
        {
            // Set the ray length
            float rayLength = playerData.groundRadius * 2.5f;

            // Send raycasts from both sides
            RaycastHit2D frontGroundHit = Physics2D.Raycast(frontRaycast, Vector2.down, rayLength, playerData.groundLayer);
            RaycastHit2D backGroundHit = Physics2D.Raycast(backRaycast, Vector2.down, rayLength, playerData.groundLayer);

            // Return under specific slope directions
            switch(slopeDirection)
            {
                case SlopeDirection.AscentFromGround:
                    return false;

                case SlopeDirection.DescentToGround:
                    return false;
            }

            // Return if either hit
            if (frontGroundHit || backGroundHit)
            {
                return true;
            }

            // Return false by default
            return false;
        }

        /// <summary>
        /// Check if the Player is bouncing
        /// </summary>
        /// <returns></returns>
        public bool CheckIfBouncing()
        {
            return isBouncing;
        }

        /// <summary>
        /// Check if the Player is against a wall
        /// </summary>
        /// <returns></returns>
        public bool CheckIfWalled()
        {
            return Physics2D.OverlapCircle(wallCheck.position, playerData.mushroomWallRadius, playerData.mushroomWallLayer);
        }

        /// <summary>
        /// Check if the Player is wall jumping
        /// </summary>
        /// <returns></returns>
        public bool CheckIfWallJumping()
        {
            return isWallJumping;
        }

        /// <summary>
        /// Get the X-direction from the Player to the Wall
        /// </summary>
        /// <returns></returns>
        public float CheckWallDirection()
        {
            // Check if walled
            if (CheckIfWalled())
            {
                // Get the wall being collided with
                Collider2D wall = Physics2D.OverlapCircle(wallCheck.position, playerData.mushroomWallRadius, playerData.mushroomWallLayer);

                // Get the direction to the wall
                Vector2 dirToWall = (wall.transform.position - transform.position).normalized;

                // Return the x direction
                return (Mathf.Abs(dirToWall.x)) / dirToWall.x;
            }
            else return 0f;
        }

        /// <summary>
        /// Check if the Player is on a Slope
        /// </summary>
        /// <returns>True if on a Slope, false if not</returns>
        public bool CheckIfOnSlope()
        {
            // Raycast to the ground from front and back
            RaycastHit2D frontGroundHit = Physics2D.Raycast(frontRaycast, Vector2.down, playerData.slopeCheckDist, playerData.groundLayer);
            RaycastHit2D backGroundHit = Physics2D.Raycast(backRaycast, Vector2.down, playerData.slopeCheckDist, playerData.groundLayer);

            // Raycast to any slopes from front and back
            RaycastHit2D frontSlopeHit = Physics2D.Raycast(frontRaycast, Vector2.down, playerData.slopeCheckDist, playerData.slopeLayer);
            RaycastHit2D backSlopeHit = Physics2D.Raycast(backRaycast, Vector2.down, playerData.slopeCheckDist, playerData.slopeLayer);

            // Check if there's a front slope hit and a back ground hit
            if(frontSlopeHit && backGroundHit)
            {
                // Get the points of contact
                Vector2 frontContact = frontSlopeHit.point;
                Vector2 backContact = backGroundHit.point;

                // If the slope point is higher than the ground, the
                // player is ascending
                if (frontContact.y > backContact.y)
                {
                    SlopeState.Direction = SlopeDirection.AscentFromGround;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(frontSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = frontSlopeHit.point;
                    return true;
                }
                else if (frontContact.y < backContact.y) // Otherwise, we are descending
                {
                    SlopeState.Direction = SlopeDirection.DescentFromGround;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(frontSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = frontSlopeHit.point;
                    return false;
                }
            }
            else if(frontSlopeHit && backSlopeHit) // Check for a front slope hit and a back slope hit
            {
                // Get points of contact
                Vector2 frontContact = frontSlopeHit.point;
                Vector2 backContact = backSlopeHit.point;

                // If the front point is higher than the back point, the player is ascending
                if(frontContact.y > backContact.y)
                {
                    SlopeState.Direction = SlopeDirection.AscentOnSlope;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(frontSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = frontSlopeHit.point;
                    return true;
                } else if(frontContact.y < backContact.y) // If the back point is higher, the player is descending
                {
                    SlopeState.Direction = SlopeDirection.DescentOnSlope;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(backSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = backSlopeHit.point;
                    return true;
                }
            } else if(backSlopeHit && frontGroundHit) // Check for a back slope hit and a front ground hit
            {
                // Get points of contact
                Vector2 frontContact = frontGroundHit.point;
                Vector2 backContact = backSlopeHit.point;

                if(backContact.y > frontContact.y)  // If the slope point is higher than the ground point,
                                                    // we are descending to the ground
                {
                    SlopeState.Direction = SlopeDirection.DescentToGround;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(backSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = backSlopeHit.point;
                    return true;
                } else if(backContact.y < frontContact.y)   // If the slope point is lower than the ground point,
                                                            // the player is ascending to the ground
                {
                    SlopeState.Direction = SlopeDirection.AscentToGround;
                    slopeDirection = SlopeState.Direction;
                    SlopeState.Angle = Vector2.Angle(backSlopeHit.normal, Vector2.up);
                    SlopeState.ContactPoint = backSlopeHit.point;
                    return false;
                }
            }

            // If none of the cases are true, there's no slope
            SlopeState.Direction = SlopeDirection.None;
            slopeDirection = SlopeState.Direction;
            return false;
        }
        #endregion

        #region ANIMATION FUNCTIONS
        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishedTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
        #endregion

        #region HELPER FUNCTIONS
        /// <summary>
        /// Check the current State
        /// </summary>
        /// <returns></returns>
        private PlayerState CheckState()
        {
            return StateMachine.CurrentState;
        }

        /// <summary>
        /// Allow the Player to run
        /// </summary>
        /// <param name="lerpAmount">The amount to sooth movement by</param>
        public void Move(float lerpAmount, bool inAir = false, bool bouncing = false)
        {
            // Calculate the direction we want to move in and our desired velocity
            float targetSpeed = InputHandler.NormInputX * playerData.runMaxSpeed;

            // Reduce our control using Lerp() this smooths changes to are direction and speed
            targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

            // Calculate accelRate
            float accelRate;

            // Set specific air accelerations and deccelerations for bouncing
            if (bouncing)
            {
                playerData.accelInAir = 0.75f;
                playerData.deccelInAir = 0f;
            }
            else
            {
                playerData.accelInAir = 0.75f;
                playerData.deccelInAir = 0.75f;
            }

            // Set specific accelerations and deccelerations for moving in the air
            if(inAir)
            {
                //accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount * playerData.accelInAir : playerData.runDeccelAmount * playerData.deccelInAir;
            } else
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;
            }

            // Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - RB.velocity.x;

            // Calculate force along x-axis to apply to the player
            float movement = speedDif * accelRate;

            // Check if right against a wall
            Vector2 upperOrigin = (lastMovementDirection == -1f) ? raycastOrigins.middleLeft : raycastOrigins.middleRight;
            Vector2 bottomOrigin = (lastMovementDirection == -1f) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            RaycastHit2D upperWallHit = Physics2D.Raycast(upperOrigin, Vector2.right * lastMovementDirection, wallCheckDist, playerData.wallLayer);
            RaycastHit2D lowerWallHit = Physics2D.Raycast(bottomOrigin, Vector2.right * lastMovementDirection, wallCheckDist, playerData.wallLayer);
            RaycastHit2D upperGroundHit = Physics2D.Raycast(upperOrigin, Vector2.right * lastMovementDirection, wallCheckDist, playerData.groundLayer);
            RaycastHit2D lowerGroundHit = Physics2D.Raycast(bottomOrigin, Vector2.right * lastMovementDirection, wallCheckDist, playerData.groundLayer);

            // If not against a wall, add force - prevents the player from sticking
            if (upperWallHit || lowerWallHit || upperGroundHit || lowerGroundHit)
            {
                RB.sharedMaterial = noFriction;
            } else
            {
                RB.sharedMaterial = null;
            }

            // Convert this to a vector and apply to rigidbody
            RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

            // Turn the player
            CheckDirectionToFace(InputHandler.NormInputX > 0);
        }

        /// <summary>
        /// Turn the Player to face the direction they are moving in
        /// </summary>
        public void CheckDirectionToFace(bool isMovingRight)
        {
            if (isMovingRight != isFacingRight)
            {
                // Stores scale and flips the player along the x axis, 
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;

                isFacingRight = !isFacingRight;
            }
        }

        /// <summary>
        /// Turn the Player to face the direction they are moving in
        /// </summary>
        public void Turn()
        {
            // Stores scale and flips the player along the x axis, 
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            isFacingRight = !isFacingRight;
        }

        /// <summary>
        /// Set the gravity scale of the Player
        /// </summary>
        /// <param name="scale"></param>
        public void SetGravityScale(float scale)
        {
            RB.gravityScale = scale;
        }

        /// <summary>
        /// End the bounce
        /// </summary>
        public void EndBounce()
        {
            isBouncing = false;
        }

        /// <summary>
        /// End the Wall Jump
        /// </summary>
        public void EndWallJump()
        {
            isWallJumping = false;
        }
        #endregion

        #region EVENT FUNCTIONS
        /// <summary>
        /// Send the PlayerController out as a reference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ReturnPlayerController(Component sender, object data)
        {
            if (sender is ThrowLine) ((ThrowLine)sender).SetPlayerController(this);
        }

        /// <summary>
        /// Check if the Player is on a wall and send the results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ReturnWallData(Component sender, object data)
        {
            if(sender is PlayerInputHandler)
            {
                if (CheckState() is PlayerWallState)
                {
                    ((PlayerInputHandler)sender).OnWall = true;
                    ((PlayerInputHandler)sender).WallDirection = CheckWallDirection();
                    ((PlayerInputHandler)sender).WallCheckPos = wallCheck.position;
                } else
                {
                    ((PlayerInputHandler)sender).OnWall = false;
                    ((PlayerInputHandler)sender).WallDirection = 0f;
                    ((PlayerInputHandler)sender).WallCheckPos = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Return data requested by MushroomThrow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void ReturnThrowData(Component sender, object data)
        {
            // Check if the sender is the correct type
            if (sender is not ThrowLine) return;

            ((ThrowLine)sender).SetFacingRight(isFacingRight);
        }

        /// <summary>
        /// Start a Mushroom Bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void StartBounce(Component sender, object data)
        {
            // Check if the correct data was sent
            if (data is not MushroomBounce.BounceData) return;
            if (StateMachine.CurrentState is PlayerJumpState) return;

            // Cast data
            MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;

            // Set whether or not the shroom is rotated
            BounceState.Rotated = bounceData.Rotated;

            // Set bounce variables
            BounceState.BounceVector = bounceData.BounceVector;
            BounceState.ForceMode = bounceData.ForceMode;

            // Set bouncing to true
            isBouncing = true;
        }

        /// <summary>
        /// Start a Wall Jump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void StartWallJump(Component sender, object data)
        {
            // Check the correct data was sent or if the wall jump is unlocked
            if (data is not MushroomWallJump.BounceData) return;

            // Set wall jumping to true
            isWallJumping = true;

            // Cast data
            MushroomWallJump.BounceData bounceData = (MushroomWallJump.BounceData)data;

            // Clear the velocity
            RB.velocity = Vector2.zero;

            // Add the force
            RB.AddForce(bounceData.BounceVector, bounceData.ForceMode);
        }

        /// <summary>
        /// Update all raycasts
        /// </summary>
        public void UpdateRaycasts()
        {
            raycastOrigins.bottomLeft = new Vector2(
                (PlayerCollider.transform.position.x - PlayerCollider.offset.x) - colliderSize.x,
                (PlayerCollider.transform.position.y + PlayerCollider.offset.y) - colliderSize.y
            );

            raycastOrigins.bottomRight = new Vector2(
                (PlayerCollider.transform.position.x + PlayerCollider.offset.x) + colliderSize.x,
                (PlayerCollider.transform.position.y + PlayerCollider.offset.y) - colliderSize.y
            );

            raycastOrigins.middleLeft = new Vector2(
                (PlayerCollider.transform.position.x - PlayerCollider.offset.x) - colliderSize.x,
                (PlayerCollider.transform.position.y + PlayerCollider.offset.y)
            );

            raycastOrigins.middleRight = new Vector2(
                (PlayerCollider.transform.position.x + PlayerCollider.offset.x) + colliderSize.x,
                (PlayerCollider.transform.position.y + PlayerCollider.offset.y)
            );

            // Set directional raycasts
            if(isFacingRight)
            {
                frontRaycast = raycastOrigins.bottomRight;
                backRaycast = raycastOrigins.bottomLeft;
            }
            else
            {
                frontRaycast = raycastOrigins.bottomLeft;
                backRaycast = raycastOrigins.bottomRight;
            }
        }
        #endregion

        private void OnDrawGizmos()
        {
            // Draw ground check
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, playerData.groundRadius);

            // Draw wall check
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wallCheck.position, playerData.mushroomWallRadius);

            // Draw slope checks
            Gizmos.color = (isFacingRight) ? Color.red : Color.blue;
            Gizmos.DrawLine(raycastOrigins.bottomLeft, new Vector2(raycastOrigins.bottomLeft.x, raycastOrigins.bottomLeft.y - playerData.slopeCheckDist));

            Gizmos.color = (isFacingRight) ? Color.blue : Color.red;
            Gizmos.DrawLine(raycastOrigins.bottomRight, new Vector2(raycastOrigins.bottomRight.x, raycastOrigins.bottomRight.y - playerData.slopeCheckDist));
        }
    }
}
