using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class ShroomTimer : MonoBehaviour
    {
        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onRemoveShroom;
        #endregion

        #region FIELDS
        [SerializeField] private float durationTimer;
        [SerializeField] private float bounceBuffer;
        [SerializeField] private bool onCooldown;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            durationTimer = 3.0f;
            bounceBuffer = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            // Update shroom duration
            UpdateShroomDuration();
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
            if (durationTimer <= 0.0f)
            {
                // Remove the shroom
                // Calls to:
                //  - MushroomTracker.RemoveShroom()
                onRemoveShroom.Raise(this, gameObject);
                return;
            }

            // Decrease time from the timer
            durationTimer -= Time.deltaTime;
        }
    }
}
