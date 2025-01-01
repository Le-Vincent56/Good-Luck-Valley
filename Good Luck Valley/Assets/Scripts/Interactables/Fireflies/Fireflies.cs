using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Interactables.Fireflies.States;
using GoodLuckValley.World.Physics;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Fireflies : Interactable, IPhysicsObject
    {
        private Transform target;
        private PhysicsOrchestrator physicsOrchestrator;

        [Header("Movement")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float currentSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float accelerationTime = 1f; // Time to reach max speed
        [SerializeField] private float decelerationTime = 0.25f;
        [SerializeField] private float decelerationDistance = 3f; // Distance to start decelerating

        [Header("Checks")]
        [SerializeField] private int channel;
        [SerializeField] private bool pendingPlacement;
        [SerializeField] private bool placed;

        private StateMachine stateMachine;

        public Transform Target { get => target; }

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new FireflyStrategy(this);
        }

        private void Start()
        {
            // Get the Physics Orchestrator
            physicsOrchestrator = ServiceLocator.ForSceneOf(this).Get<PhysicsOrchestrator>();

            // Set up the State Machine
            SetupStateMachine();
        }

        private void Update()
        {
            // Update the State Machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Update the State Machine
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Tick the Physics Update for the Fireflies
        /// </summary>
        public void TickUpdate(float delta, float time) 
        {
            // Exit case - there's no Target
            if (target == null) return;

            MoveTowardsTarget(delta);
        }

        /// <summary>
        /// Tick the Fixed Physics Update for the Fireflies
        /// </summary>
        public void TickFixedUpdate(float delta) { }

        /// <summary>
        /// Move towards the Target
        /// </summary>
        /// <param name="delta"></param>
        private void MoveTowardsTarget(float delta)
        {
            // Calculate the direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calculate distance to the target
            float distance = Vector2.Distance(transform.position, target.position);

            // Adjust speed based on acceleration or deceleration
            if (distance > decelerationDistance)
            {
                // Accelerate to max speed
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, delta / accelerationTime);
            }
            else if(distance >= 0.1f)
            {
                // Decelerate as it approaches the target
                float targetSpeed = Mathf.Lerp(0, maxSpeed, distance / decelerationDistance);
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, delta / accelerationTime);

                Debug.Log($"Decelerating: " +
                    $"\nTarget Speed: {targetSpeed}" +
                    $"\nCurrent Speed: {currentSpeed}");
            }

            // Move the object
            transform.position += currentSpeed * delta * direction;

            // Check if the object has reached the target
            if (distance < 0.1f)
            {
                // Set the current speed to 0 and set placed
                currentSpeed = 0;
            }
        }

        /// <summary>
        /// Set up the State Machine for the Fireflies
        /// </summary>
        private void SetupStateMachine()
        {
            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Declare states
            GroupIdleState idleState = new GroupIdleState(this);
            GroupLocomotionState locomotionState = new GroupLocomotionState(this, physicsOrchestrator);

            // Define state transitions
            stateMachine.At(idleState, locomotionState, new FuncPredicate(() => target != null));
            stateMachine.At(locomotionState, idleState, new FuncPredicate(() => target == null && placed));

            // Set an initial state
            stateMachine.SetState(idleState);
        }
        
        /// <summary>
        /// Interact with the Fireflies
        /// </summary>
        public override void Interact()
        {
            // Exit case - the Handler has no value
            if (!handler.HasValue) return;

            // Exit case - the Interactable cannot be interaacted with
            if (!canInteract) return;

            // Exit case - the Interactable Strategy fails
            if (!strategy.Interact(handler.Value)) return;

            // Remove the Interactable from the Handler
            handler.Match(
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Set un-interactable
            canInteract = false;

            // Fade out the sprites and deactivate
            FadeFeedback(0f, fadeDuration);
        }

        /// <summary>
        /// Set a target for the Fireflies to follow
        /// </summary>
        public void Follow(Transform target)
        {
            this.target = target;
            Debug.Log($"Firefly Target Set: {this.target}");
        }

        /// <summary>
        /// Release the Fireflies from following a target
        /// </summary>
        public void Release() => target = null;
    }
}
