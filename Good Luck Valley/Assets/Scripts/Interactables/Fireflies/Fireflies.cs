using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Interactables.Fireflies.States;
using GoodLuckValley.World.Physics;
using UnityEngine;
using static Sirenix.OdinInspector.Editor.Internal.FastDeepCopier;

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
        [SerializeField] private float acceleration = 2f;  // Acceleration
        [SerializeField] private float deceleration = 2f; // Deceleration
        [SerializeField] private float stoppingDistance = 3f; // Distance to start decelerating

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

            // Set variables
            velocity = Vector2.zero;
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
            // Calculate the direction towards the target
            Vector2 direction = (target.position - transform.position).normalized;
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // If we are not at the target, start moving
            if (distanceToTarget > stoppingDistance)
            {
                // Accelerate until reaching max speed
                if (velocity.magnitude < maxSpeed)
                {
                    velocity += direction * acceleration * delta;
                    velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
                }
                else
                {
                    velocity = direction * maxSpeed;
                }
            }
            else
            {
                // Decelerate to a stop
                velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * delta);
            }

            // Move the object based on the velocity
            transform.position += (Vector3)velocity * delta;
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
