using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyGroup : MonoBehaviour
    {
        private Vector2 circleCenter;
        [SerializeField] private bool moving;
        [SerializeField] private float circleRadius = 10f;
        private float time;
        private float delta;

        private List<Firefly> fireflies;

        public bool Moving { get => moving; set => moving = value; } 
        public Vector2 Center { get => circleCenter; }

        private void Start()
        {
            fireflies = new List<Firefly>();

            // Get all the Fireflies within the group
            GetComponentsInChildren(fireflies);

            // Set the circle center
            circleCenter = (Vector2)transform.position;

            // Iterate through each Firefly and set the native data
            for(int i = 0; i < fireflies.Count; i++)
            {
                // Initialize the Firefly
                fireflies[i].Initialize(this);
            }

            // Set variables
            moving = false;
        }

        private void Update()
        {
            // Get the time and delta time
            time = Time.time;
            delta = Time.deltaTime;

            // Iterate through each Firefly
            foreach (Firefly firefly in fireflies)
            {
                // Update the Firefly
                firefly.TickUpdate(time, delta, fireflies);
            }

            // Exit case - the circle center is already the transform's position
            if (circleCenter == (Vector2)transform.position)
            {
                // If moving, set to not moving
                if (moving) moving = false;

                return;
            }

            // Udpate the circle center
            circleCenter = (Vector2)transform.position;

            // Set to moving
            moving = true;
        }

        /// <summary>
        /// Get a random position within the circle
        /// </summary>
        public Vector2 GetRandomPositionInCircle() => circleCenter + GetRandomOffset();

        /// <summary>
        /// Get a random offset within the circle
        /// </summary>
        public Vector2 GetRandomOffset()
        {
            // Get a random direction within the circle
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Get a random distance
            float randomDistance = Random.Range(0.1f, circleRadius);

            return randomDirection * randomDistance;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleRadius);
        }
    }
}
