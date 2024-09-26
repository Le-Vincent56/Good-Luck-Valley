using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies
{
    public class FireflyGroup : MonoBehaviour
    {
        private Vector2 circleCenter;
        [SerializeField] private float circleRadius = 10f;

        private Firefly[] fireflies;

       private void Start()
        {
            // Get the fireflies
            fireflies = GetComponentsInChildren<Firefly>();

            // Iterate  through each Firefly
            for(int i = 0; i < fireflies.Length; i++)
            {
                // Set the this as the Firefly group
                fireflies[i].SetFireflyGroup(this);
            }

            circleCenter = transform.position;
        }

        public bool IsWithinCircle(Vector2 position) => Vector2.Distance(position, circleCenter) <= circleRadius;

        /// <summary>
        /// Get a random position within the circle
        /// </summary>
        public Vector2 GetRandomPositionInCircle()
        {
            // Get a random direction within the circle
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Get a random distance
            float randomDistance = Random.Range(0.1f, circleRadius);

            return circleCenter + randomDirection * randomDistance;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleRadius);
        }
    }
}