using GoodLuckValley.Events;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.Patterns.Visitor;
using GoodLuckValley.World.Interactables;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class FireflyController : MonoBehaviour, IVisitor
    {
        [Header("Events")]
        [SerializeField] private GameEvent onGetPlayerTransform;
        [SerializeField] private GameEvent onSetPlayerFireflies;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private DynamicCircleCollisionHandler collisionHandler;
        [SerializeField] private Transform followTarget;
        [SerializeField] private Lantern lastLantern;
        [SerializeField] private Vector2 retreatPosition;

        [Header("Fields - Movement")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float movementSpeed;

        [Header("Fields - Checks")]
        [SerializeField] private int channel;
        [SerializeField] private bool isTrapped;
        [SerializeField] private bool isPlaced;
        [SerializeField] private bool isRetreating;
        [SerializeField] private bool isPendingPlaced;
        [SerializeField] private bool playerCanTakeFireflies;

        private StateMachine stateMachine;

        public bool IsPendingPlaced { get { return isPendingPlaced; } }

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<DynamicCircleCollisionHandler>();

            // Declare states
            stateMachine = new StateMachine();
            GroupIdleState idleState = new GroupIdleState(this, animator);
            GroupFollowState followState = new GroupFollowState(this, animator);
            GroupRetreatState retreatState = new GroupRetreatState(this, animator);

            // Define strict transitions
            At(idleState, followState, new FuncPredicate(() => followTarget != null));
            At(followState, retreatState, new FuncPredicate(() => isRetreating));
            At(followState, idleState, new FuncPredicate(() => isPendingPlaced && transform.position == followTarget.position));
            At(retreatState, idleState, new FuncPredicate(() => followTarget == null && !isRetreating));

            stateMachine.SetState(idleState);
        }

        private void Start()
        {
            retreatPosition = transform.position;
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        public void HandleMovement()
        {
            Move(velocity * Time.deltaTime);

            if (collisionHandler.collisions.Collision) isRetreating = true;
        }

        /// <summary>
        /// Move the fireflies
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="standingOnPlatform"></param>
        public void Move(Vector2 velocity, bool standingOnPlatform = false)
        {
            collisionHandler.UpdateRaycastOrigins();

            collisionHandler.collisions.ResetInfo();

            collisionHandler.UpdateCollisions(ref velocity);

            transform.position = Vector2.MoveTowards(transform.position, followTarget.position, movementSpeed);
        }

        public void Free(bool isFree)
        {
            // Check if the fireflies are free or not
            if (!isFree)
            {
                // If there's no current target, the fireflies are trapped
                if (followTarget == null && lastLantern == null)
                    isTrapped = true;

                return;
            }

            // If already freed, don't get triggered by the decompose controller
            if (!isTrapped) return;

            // Set is trapped to false and set the current target
            isTrapped = false;

            // Get the player transform to set
            // Calls to:
            //  - PlayerController.OnGetPlayerTransform()
            onGetPlayerTransform.Raise(this, null);
        }

        /// <summary>
        /// Set the follow target
        /// </summary>
        /// <param name="transform"></param>
        public void SetFollowTarget(Transform transform)
        {
            // If trapped, return
            if (isTrapped) return;

            // Otherwise, set the follow target
            followTarget = transform;
        }

        public void Visit<T>(T visitable) where T : Component, IVisitable
        {
            // Check if the correct type was sent
            if(visitable is Lantern lantern)
            {
                // Set the follow target if not retreating
                if(!isRetreating)
                {
                    followTarget = lantern.gameObject.transform;
                    isPendingPlaced = true;
                }
                
                // Set this lantern as the last lantern
                lastLantern = lantern;
            }
        }

        public void OnStoreFireflies(Component sender, object data)
        {
            if (sender is not Lantern) return;
            if (data is not int) return;
            if ((int)data != channel) return;
            if (isTrapped || isPlaced || isPendingPlaced) return;

            // Visit the lantern
            Visit((Lantern)sender);
        }

        public void OnWithdrawFireflies(Component sender, object data)
        {
            if (sender is not Lantern) return;
            if (!isPlaced || isPendingPlaced) return;
            if (!playerCanTakeFireflies) return;
            if (data is not int) return;
            if ((int)data != channel) return;

            ((Lantern)sender).Withdraw();
            isPlaced = false;
            onGetPlayerTransform.Raise(this, null);
        }

        public void SetRetreatTarget()
        {
            retreatPosition = followTarget.position;
            followTarget = null;
        }

        public void Retreat()
        {
            // Remove any follow target
            followTarget = null;

            if(lastLantern != null)
            {
                isTrapped = false;
                isPlaced = true;

                lastLantern.Accept(this);
            } else
            {
                isTrapped = true;
                isPendingPlaced = false;
                isPlaced = false;
            }

            onSetPlayerFireflies.Raise(this, false);
            transform.position = retreatPosition;
            isRetreating = false;
        }

        public void CheckPlaced()
        {
            if (isPendingPlaced)
            {
                // Have the lantern accept the fireflies
                lastLantern.Accept(this);

                // Set placed variables
                isPendingPlaced = false;
                isPlaced = true;

                // Set whether or not the player has fireflies
                onSetPlayerFireflies.Raise(this, false);
            }
        }

        public void PlayerHasFireflies(bool hasFireflies)
        {
            playerCanTakeFireflies = !hasFireflies;
        }
    }
}