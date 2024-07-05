using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AreaCollider : MonoBehaviour
    {
        public enum Type
        {
            Box,
            Composite
        }

        public UnityAction<GameObject> OnTriggerEnter;
        public UnityAction<GameObject> OnTriggerExit;

        protected BoxCollider2D entityCollider;
        protected CompositeCollider2D compositeCollider;

        [Header("Fields")]
        [SerializeField] private Type colliderType;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Bounds bounds;
        [SerializeField] private HashSet<GameObject> currentCollisions = new HashSet<GameObject>();

        public Bounds Bounds { get { return bounds; } }

        private void Reset()
        {
            // Set the "Player" as a default layer when the component is added
            layerMask = 1024;
        }

        private void Awake()
        {
            switch (colliderType)
            {
                case Type.Box:
                    entityCollider = GetComponent<BoxCollider2D>();
                    bounds = entityCollider.bounds;
                    break;

                case Type.Composite:
                    compositeCollider = GetComponent<CompositeCollider2D>();
                    bounds = compositeCollider.bounds;
                    break;
            }
        }

        private void Update()
        {
            // Check collisions for every frame
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0, layerMask);

            HashSet<GameObject> newCollisions = new HashSet<GameObject>();

            // Loop through all of the new collisions
            foreach(Collider2D hit in hits)
            {
                // Add the hit game object to the hashset
                newCollisions.Add(hit.gameObject);

                // If the current collisions don't contain the new object,
                // it means something has entered the trigger, so invoke the event
                if (!currentCollisions.Contains(hit.gameObject))
                    OnTriggerEnter?.Invoke(hit.gameObject);
            }

            // Loop through all of the current collisions
            foreach (GameObject gameObj in currentCollisions)
            {
                // If the new collisions hashset doesn't contain the current collision,
                // it has left, so invoke the event
                if(!newCollisions.Contains(gameObj))
                    OnTriggerExit?.Invoke(gameObj);
            }

            // Set the new collisions hash set
            currentCollisions = newCollisions;
        }

        private void OnDrawGizmos()
        {
            if (entityCollider == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}