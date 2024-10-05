using GoodLuckValley.Entity;
using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class FacingDirectionTrigger : MonoBehaviour
    {
        [Range(-1f, 1f)]
        [SerializeField] private int direction;
        private AreaCollider areaCollider;

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += TriggerEnter;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= TriggerEnter;
        }

        private void TriggerEnter(GameObject other)
        {
            // Extract the collision handler
            DynamicCollisionHandler collisionHandler = other.GetComponent<DynamicCollisionHandler>();

            // Set the facing direction
            collisionHandler.SetFacingDirection(direction);
        }
    }
}
