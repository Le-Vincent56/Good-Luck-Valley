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
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float decelerationDistance;
        [SerializeField] private float bounceSpeed;
        [SerializeField] private float bounceAmplitude;

        private StateMachine stateMachine;

        public bool Idle { get => idle; set => idle = value; }
        public bool Wandering { get => wandering; set => wandering = value; }

        public FireflyGroup Group { get => group; }

        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float Acceleration { get => acceleration; set => acceleration = value; }
        public float DecelerationDistance { get => decelerationDistance; set => decelerationDistance = value; }
        public float BounceSpeed { get => bounceSpeed; set => bounceSpeed = value; }
        public float BounceAmplitude { get => bounceAmplitude; set => bounceAmplitude = value; }

        /// <summary>
        /// Update the Firefly
        /// </summary>
        public void TickUpdate()
        {
            // Update the state machine
            stateMachine.Update();
        }

        /// <summary>
        /// Initialize the Firefly
        /// </summary>
        public void Initialize(FireflyGroup group)
        {
            // Set variables
            this.group = group;

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
            FireflyWanderState wanderState = new FireflyWanderState(this, maxSpeed, acceleration, decelerationDistance, bounceSpeed, bounceAmplitude);
            FireflyLocomotionState moveState = new FireflyLocomotionState(this, maxSpeed, acceleration, decelerationDistance);

            // Define state transitions
            stateMachine.At(moveState, wanderState, new FuncPredicate(() => wandering && !group.Moving));
            stateMachine.Any(moveState, new FuncPredicate(() => group.Moving));

            // Set the initial state
            stateMachine.SetState(wanderState);
        }
    }
}
