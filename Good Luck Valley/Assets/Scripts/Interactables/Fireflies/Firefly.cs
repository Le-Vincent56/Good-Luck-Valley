using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Interactables.Fireflies.States;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Firefly : MonoBehaviour
    {
        private FireflyGroup group;

        [Header("Movement")]
        [SerializeField] private bool move;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float decelerationDistance;

        [Header("Idle")]
        [SerializeField] private float waitTimeMin;
        [SerializeField] private float waitTimeMax;

        private StateMachine stateMachine;

        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float Acceleration { get => acceleration; set => acceleration = value; }
        public float DecelerationDistance { get => decelerationDistance; set => decelerationDistance = value; }
        public float WaitTimeMin { get => waitTimeMin; set => waitTimeMin = value; }
        public float WaitTimeMax { get => waitTimeMax; set => waitTimeMax = value; }

        private void Update()
        {
            // Update the state machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Fixed update the state machine
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Initialize the Firefly
        /// </summary>
        public void Initialize(FireflyGroup group)
        {
            // Set variables
            this.group = group;
            move = false;

            SetupStateMachine();
        }
        
        /// <summary>
        /// Set up the State Machine
        /// </summary>
        private void SetupStateMachine()
        {
            stateMachine = new StateMachine();

            // Create states
            FireflyIdleState idleState = new FireflyIdleState(this, waitTimeMin, waitTimeMax);
            FireflyLocomotionState moveState = new FireflyLocomotionState(this, maxSpeed, acceleration, decelerationDistance);

            // Define state transitions
            stateMachine.At(idleState, moveState, new FuncPredicate(() => move && !group.Moving));
            stateMachine.At(moveState, idleState, new FuncPredicate(() => !move && !group.Moving));

            // Set the initial state
            stateMachine.SetState(idleState);
        }

        /// <summary>
        /// Calculate a new movement position for the Firefly
        /// </summary>
        public Vector2 GetNewPosition() => group.GetRandomPositionInCircle();

        /// <summary>
        /// Set whether or not the Firefly has reached its destination
        /// </summary>
        public void SetToMove(bool move) => this.move = move;
    }
}
