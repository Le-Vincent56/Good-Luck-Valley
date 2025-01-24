using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyGroup : MonoBehaviour
    {
        private Vector2 circleCenter;
        [SerializeField] private bool moving;
        [SerializeField] private float circleRadius = 10f;

        private Firefly[] fireflies;

        public bool Moving { get => moving; set => moving = value; } 

        private void Start()
        {
            // Get all the Fireflies within the group
            fireflies = GetComponentsInChildren<Firefly>();

            // Iterate through each Firefly
            for (int i = 0; i < fireflies.Length; i++)
            {
                // Set the this as the Firefly group
                fireflies[i].Initialize(this);
            }

            // Set the circle center
            circleCenter = (Vector2)transform.position;

            // Set variables
            moving = false;
        }

        private void Update()
        {
            // Iterate through each Firefly
            foreach(Firefly firefly in fireflies)
            {
                // Update the Firefly
                firefly.TickUpdate();
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

        /// <summary>
        /// Get the position within the circle using an offset
        /// </summary>
        public Vector2 GetOffsetPosition(Vector2 offset) => circleCenter + offset;

        private void OnDrawGizmosSelected()
        {
            // Draw the circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, circleRadius);
        }
    }
}
