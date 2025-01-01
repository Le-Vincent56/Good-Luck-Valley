using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Interactables.Fireflies.States;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Firefly : MonoBehaviour
    {
        private FireflyGroup group;

        [Header("Movement")]
        [SerializeField] private bool idle;
        [SerializeField] private bool wandering;
        [SerializeField] private bool move;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float decelerationDistance;

        [Header("Idle")]
        [SerializeField] private float waitTimeMin;
        [SerializeField] private float waitTimeMax;

        private StateMachine stateMachine;

        public bool Idle { get => idle; set => idle = value; }
        public bool Wandering { get => wandering; set => wandering = value; }

        public FireflyGroup Group { get => group; }

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

            // Set up the State Machine
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
            FireflyWanderState wanderState = new FireflyWanderState(this, maxSpeed, acceleration, decelerationDistance);
            FireflyLocomotionState moveState = new FireflyLocomotionState(this, maxSpeed, acceleration, decelerationDistance);

            // Define state transitions
            stateMachine.At(idleState, wanderState, new FuncPredicate(() => wandering && !group.Moving));
            stateMachine.At(wanderState, idleState, new FuncPredicate(() => idle && !group.Moving));
            stateMachine.At(moveState, idleState, new FuncPredicate(() => idle && !group.Moving));
            stateMachine.Any(moveState, new FuncPredicate(() => group.Moving));

            // Set the initial state
            stateMachine.SetState(idleState);
        }

        /// <summary>
        /// Set whether or not the Firefly has reached its destination
        /// </summary>
        public void SetToMove(bool move) => this.move = move;
    }
}
