using GoodLuckValley.Player.StateMachine.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine
{
    public class Player : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] string previousState;
        [SerializeField] string currentState;
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerInAirState InAirState { get; private set; }
        public PlayerLandState LandState { get; private set; }

        public Rigidbody2D RB { get; private set; }
        public BoxCollider2D PlayerCollider { get; private set; }
        public Animator Anim { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }

        [SerializeField] private PlayerData playerData;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform leftWallCheck;
        [SerializeField] private Transform rightWallCheck;
        [SerializeField] private LayerMask groundLayer;

        public bool isLocked = false;
        private bool isFacingRight = true;
        #endregion

        private void Awake()
        {
            // Create a new state machine
            StateMachine = new PlayerStateMachine();

            IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
            MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
            JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
            InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
            LandState = new PlayerLandState(this, StateMachine, playerData, "land");
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
        }

        private void FixedUpdate()
        {
            // Run physics updates for the current state
            StateMachine.CurrentState.PhysicsUpdate();
        }

        #region CHECK FUNCTIONS
        /// <summary>
        /// Check if the player is grounded
        /// </summary>
        /// <returns>True if the player is grounded, false if not</returns>
        public bool CheckIfGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, playerData.groundRadius, playerData.groundLayer);
        }
        #endregion

        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
        private void AnimationFinishedTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

        /// <summary>
        /// Allow the Player to Run
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
        /// Set the gravity scale of the player
        /// </summary>
        /// <param name="scale"></param>
        public void SetGravityScale(float scale)
        {
            RB.gravityScale = scale;
        }
    }
}
