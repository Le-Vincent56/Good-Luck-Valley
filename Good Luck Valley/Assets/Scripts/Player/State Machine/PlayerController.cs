using GoodLuckValley.Events;
using GoodLuckValley.UI;
using GoodLuckValley.Player.StateMachine.States;
using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class PlayerController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private PlayerData playerData;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform leftWallCheck;
        [SerializeField] private Transform rightWallCheck;
        [SerializeField] private LayerMask groundLayer;
        #endregion

        #region FIELDS
        [SerializeField] string previousState;
        [SerializeField] string currentState;

        public bool isLocked = false;
        private bool isFacingRight = true;
        private bool isBouncing = false;
        #endregion

        #region PROPERTIES
        public Rigidbody2D RB { get; private set; }
        public BoxCollider2D PlayerCollider { get; private set; }
        public Animator Anim { get; private set; }
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
        }

        private void Start()
        {
            // Get components
            RB = GetComponent<Rigidbody2D>();
            PlayerCollider = GetComponent<BoxCollider2D>();
            Anim = GetComponent<Animator>();
            InputHandler = GetComponent<PlayerInputHandler>();

            // Enter the idle state
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            // Run the logic updates for the current state
            StateMachine.CurrentState.LogicUpdate();

            // Check and set states
            if(StateMachine.PreviousState != null)
                previousState = StateMachine.PreviousState.ToString().Substring(48);
            currentState = StateMachine.CurrentState.ToString().Substring(48);

            // Check to reset jump input
            if(InputHandler.JumpInput && 
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
        /// <returns>True if the player is grounded, false if not</returns>
        public bool CheckIfGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, playerData.groundRadius, playerData.groundLayer);
        }

        /// <summary>
        /// Check if the Player is bouncing
        /// </summary>
        /// <returns></returns>
        public bool CheckIfBouncing()
        {
            return isBouncing;
        }
        #endregion

        #region ANIMATION FUNCTIONS
        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishedTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
        #endregion

        #region HELPER FUNCTIONS
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

        public void StartBounce(Component sender, object data)
        {
            // Check if the correct data was sent
            if (data is not MushroomBounce.BounceData) return;
            if (StateMachine.CurrentState is PlayerJumpState) return;

            // Set bouncing to true
            isBouncing = true;

            // Cast data
            MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;
            
            // Clear the velocity
            RB.velocity = Vector2.zero;

            // Add the force
            RB.AddForce(bounceData.BounceVector, bounceData.ForceMode);
        }

        /// <summary>
        /// End the bounce
        /// </summary>
        public void EndBounce()
        {
            isBouncing = false;
        }
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, playerData.groundRadius);
        }
    }
}
