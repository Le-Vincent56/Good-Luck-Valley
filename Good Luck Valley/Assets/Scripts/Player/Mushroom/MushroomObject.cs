using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.Mushroom
{
    public class MushroomObject : MonoBehaviour
    {
        private Animator animator;
        private StateMachine stateMachine;
        private ParticleSystem particles;
        private MushroomSFX sfx;

        [Header("Status")]
        [SerializeField] private bool bounceEntity;
        [SerializeField] private bool growing;
        [SerializeField] private bool dissipating;

        /// <summary>
        /// Initialize the Mushroom Object
        /// </summary>
        public void Initialize()
        {
            // Get components
            animator = GetComponent<Animator>();
            particles = GetComponent<ParticleSystem>();
            sfx = GetComponent<MushroomSFX>();

            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Create states
            GrowState growState = new GrowState(this, animator);
            IdleState idleState = new IdleState(this, animator);
            BounceState bounceState = new BounceState(this, animator, sfx);
            DissipateState dissipateState = new DissipateState(this, animator, sfx);

            // Define state transitions
            stateMachine.At(growState, idleState, new FuncPredicate(() => !growing));
            stateMachine.At(growState, bounceState, new FuncPredicate(() => bounceEntity));
            stateMachine.At(idleState, bounceState, new FuncPredicate(() => bounceEntity));
            stateMachine.At(bounceState, idleState, new FuncPredicate(() => !bounceEntity));
            stateMachine.Any(dissipateState, new FuncPredicate(() => dissipating));

            // Set an initial state
            stateMachine.SetState(growState);
        }

        private void Update()
        {
            // Update the State Machine
            stateMachine?.Update();
        }

        private void FixedUpdate()
        {
            // Update the State Machine
            stateMachine?.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the Mushroom is dissipating
            if (dissipating) return;

            // Exit case - the colliding object is not the Player
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Get the point of contact
            Vector2 contactPoint = collision.ClosestPoint(transform.position);

            // Calculate local bounds coordinates
            Bounds bounds = GetComponent<Collider2D>().bounds;
            Vector2 localPoint = new Vector2(
                (contactPoint.x - bounds.min.x) / bounds.size.x,
                (contactPoint.y - bounds.min.y) / bounds.size.y
            );

            // Prepare a bounce
            controller.Bounce.PrepareBounce(localPoint.y);

            // Set bouncing
            bounceEntity = true;
        }

        /// <summary>
        /// Play the Mushroom particles
        /// </summary>
        public void UnleashParticles() => particles.Play();

        /// <summary>
        /// Stop growing the Mushroom
        /// </summary>
        public void StopGrowing() => growing = false;

        /// <summary>
        /// Stop Bouncing the Mushroom
        /// </summary>
        public void StopBouncing() => bounceEntity = false;

        /// <summary>
        /// Start Dissipating the Mushroom
        /// </summary>
        public void StartDissipating() => dissipating = true;

        /// <summary>
        /// Destroy the Mushroom
        /// </summary>
        public void DestroyMushroom() => Destroy(gameObject);
    }
}
