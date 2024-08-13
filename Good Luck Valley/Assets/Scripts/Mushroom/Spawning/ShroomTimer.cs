using GoodLuckValley.Events;
using GoodLuckValley.Patterns.Visitor;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class ShroomTimer : MonoBehaviour, IVisitable
    {
        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onRemoveShroom;
        #endregion

        #region FIELDS
        [SerializeField] private bool removedByTimer;
        [SerializeField] private float durationTimer;
        [SerializeField] private float bounceBuffer;
        [SerializeField] private bool onCooldown;
        [SerializeField] private bool tryingToRemove;
        [SerializeField] private bool removeCalled;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            bounceBuffer = 0.1f;
            tryingToRemove = false;
            removeCalled = false;
        }

        // Update is called once per frame
        void Update()
        {
            // Check if the duration timer should be run
            if (!removedByTimer) return;

            // Update shroom duration
            UpdateShroomDuration();

            if(tryingToRemove)
            {
                // Remove the shroom
                // Calls to:
                //  - MushroomTracker.RemoveShroom()
                onRemoveShroom.Raise(this, gameObject);

                removeCalled = true;
                tryingToRemove = false;
            }
        }

        /// <summary>
        /// Set the bounce cooldown
        /// </summary>
        public void SetCooldown()
        {
            if (onCooldown)
            {
                bounceBuffer -= Time.deltaTime;
            }

            if (bounceBuffer <= 0.0f)
            {
                onCooldown = false;
                bounceBuffer = 0.1f;
            }
        }

        /// <summary>
        /// Update the Mushroom's life duration
        /// </summary>
        public void UpdateShroomDuration()
        {
            // Check if the shroom has lasted it's duration
            if (durationTimer <= 0.0f && !removeCalled)
            {
                tryingToRemove = true;
                return;
            }

            // Decrease time from the timer
            durationTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Set the duration timer
        /// </summary>
        /// <param name="duration">The duration of the timer</param>
        public void SetDuration(float duration)
        {
            durationTimer = duration;
        }

        public void Accept<T>(T visitor) where T : Component, IVisitor
        {
            // Accept the visit and alloww the visitor to visit
            // the shroom timer
            visitor.Visit(this);
        }
    }
}
