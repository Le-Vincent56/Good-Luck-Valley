using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Events;
using GoodLuckValley.Interactables.Fireflies.States;
using GoodLuckValley.World.Physics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Fireflies : GateInteractable, IPhysicsObject
    {
        private Transform target;
        private PhysicsOrchestrator physicsOrchestrator;
        private Light2D feedbackLight;

        [Header("Movement")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float currentSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration = 1f;
        [SerializeField] private float decelerationDistance = 3f;

        [Header("Checks")]
        [SerializeField] private bool pendingPlacement;
        [SerializeField] private bool placed;

        [Header("Tweening Variables")]
        [SerializeField] private float flashDuration;
        private float toLightIntensity;
        private Tween intensityTween;

        private StateMachine stateMachine;

        public Transform Target { get => target; }

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new FireflyStrategy(this);

            feedbackLight = GetComponentInChildren<Light2D>();
            toLightIntensity = feedbackLight.intensity;
            feedbackLight.intensity = 0f;
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Kill any existing Tweens
            intensityTween?.Kill();
        }

        /// <summary>
        /// Tick the Physics Update for the Fireflies
        /// </summary>
        public void TickUpdate(float delta, float time) 
        {
            // Exit case - there's no Target
            if (target == null) return;

            // Move towards the target
            MoveTowardsTarget(delta);
        }

        /// <summary>
        /// Tick the Fixed Physics Update for the Fireflies
        /// </summary>
        public void TickFixedUpdate(float delta) { }

        /// <summary>
        /// Move towards the Target
        /// </summary>
        private void MoveTowardsTarget(float delta)
        {
            /// Get the distance to the target position
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Check if within deceleration distance
            if (distanceToTarget < decelerationDistance)
            {
                // Decelerate to the current speed
                float t = distanceToTarget / decelerationDistance;
                currentSpeed = Mathf.Lerp(0, maxSpeed, t);
            }
            else
            {
                // Accelerate the current speed
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * delta);
            }

            // Move the firefly to the target position
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                currentSpeed * delta
            );

            // Check if within stopping distance
            if (Vector2.Distance(transform.position, target.position) < 0.01f)
            {
                // Set the current speed to 0
                currentSpeed = 0f;

                // Check if pending placement
                if(pendingPlacement)
                {
                    // Finalize the placement
                    pendingPlacement = false;
                    placed = true;
                }
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
            GroupFollowState followState = new GroupFollowState(this, physicsOrchestrator);

            // Define state transitions
            stateMachine.At(idleState, followState, new FuncPredicate(() => target != null));
            stateMachine.At(followState, idleState, new FuncPredicate(() => placed));

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

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = fadeDuration
            });

            // Flash the feedback light
            Flash(toLightIntensity, flashDuration / 2f, () =>
            {
                Flash(0f, flashDuration / 2f);
            });
        }

        /// <summary>
        /// Set a target for the Fireflies to follow
        /// </summary>
        public void Follow(Transform target, bool finalDestination = false)
        {
            this.target = target;

            // Exit case - the target is not the final destination
            if (!finalDestination) return;

            pendingPlacement = true;
        }

        /// <summary>
        /// Release the Fireflies from following a target
        /// </summary>
        public void Release() => target = null;

        protected override bool CheckForKey(InteractableHandler interactableHandler)
        {
            // Set to false by default
            bool hasKey = false;

            // Check if the Firefly Handler has a fruit
            interactableHandler.FireflyHandler.GetFruit().Match(
                onValue: fruit =>
                {
                    // If the player has Fruit, they have the key
                    hasKey = true;

                    return 0;
                },
                onNoValue: () =>
                {
                    // The Firefly Handler does not have a Fruit, so do nothing
                    return 0;
                }
            );

            return hasKey;
        }

        private void Flash(float flashValue, float flashDuration, TweenCallback onComplete = null)
        {
            // Kill the Flash Sequence if it exists
            intensityTween?.Kill();

            // Set the light to the flash value
            intensityTween = DOTween.To(
                () => feedbackLight.intensity,
                    x => feedbackLight.intensity = x,
                    flashValue,
                    flashDuration / 2f
            ).SetEase(Ease.InOutSine);

            // Exit case - if there is no completion actino
            if (onComplete == null) return;

            // Hook up the completion action
            intensityTween.onComplete += onComplete;
        }
    }
}
