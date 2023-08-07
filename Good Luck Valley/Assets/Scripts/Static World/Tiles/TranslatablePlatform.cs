using UnityEngine;

namespace HiveMind.Tiles
{
    public class TranslatablePlatform : MoveablePlatform
    {
        #region FIELDS
        [Header("Movement")]
        [SerializeField] private Vector3 direction;
        [SerializeField] private Vector3 maxDistance;
        [SerializeField] private float translateSpeed;

        [SerializeField] private Vector3 initialPosition;
        private Vector3 platformPosition;
        private Vector3 moveDirection;
        private Vector3 velocity;
        #endregion

        void Start()
        {
            initialPosition = transform.position;
            platformPosition = new Vector3(transform.position.x, transform.position.y, 0);
        }

        public override void Move()
        {
            // What direction platfrom will move in
            moveDirection = direction;

            // Calculating how much time it will take platfrom to travel to new position
            velocity = moveDirection * translateSpeed * Time.deltaTime;

            // Telling platfrom how to move
            platformPosition += velocity;

            // Setting the platfroms original position to new position (moving platfrom).
            transform.position = platformPosition;

            // Move shroom with platform
            foreach (GameObject shroom in stuckShrooms)
            {
                shroom.transform.position += velocity;

            }

            if (Mathf.Abs((int)platformPosition.y) == Mathf.Abs((int)initialPosition.y - maxDistance.y)) // Platfrom has reached the max distance it can move
            {
                isTriggered = false; // Stop moving
            }
        }
    }
}