using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class ShroomTimer : MonoBehaviour
    {
        [SerializeField] private float durationTimer;
        [SerializeField] private float bounceBuffer;
        [SerializeField] private bool onCooldown;

        // Start is called before the first frame update
        void Start()
        {
            bounceBuffer = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            // Update shroom duration
            UpdateShroomDuration();
        }

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

        public void UpdateShroomDuration()
        {
            // Check if the shroom has lasted it's duration
            if (durationTimer <= 0.0f)
            {
                // Destroy the game object
                Destroy(gameObject);
                return;
            }

            // Decrease time from the timer
            durationTimer -= Time.deltaTime;
        }
    }
}
