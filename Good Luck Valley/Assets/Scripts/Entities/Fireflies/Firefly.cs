using GoodLuckValley.Entities.Fireflies.States;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
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

        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float Acceleration { get => acceleration; set => acceleration = value; }
        public float DecelerationDistance { get => decelerationDistance; set => decelerationDistance = value; }
        public float WaitTimeMin {  get => waitTimeMin; set => waitTimeMin = value; }
        public float WaitTimeMax { get => waitTimeMax; set => waitTimeMax = value; }

        private StateMachine stateMachine;

        private void Awake()
        {
            // Initialize the state machine
            stateMachine = new StateMachine();

            // Create states
            IdleState idleState = new IdleState(this, waitTimeMin, waitTimeMax);
            MoveState moveState = new MoveState(this, maxSpeed, acceleration, decelerationDistance);

            // Define state transitions
            At(idleState, moveState, new FuncPredicate(() => move));
            At(moveState, idleState, new FuncPredicate(() => !move));

            // Set an initial state
            stateMachine.SetState(idleState);

            // Set variables
            move = false;
        }

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
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Set the Firefly group
        /// </summary>
        public void SetFireflyGroup(FireflyGroup group) => this.group = group;

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