using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField] private GameEvent onToggleCinematicBars;

        public enum Type
        {
            Start,
            End
        }

        [SerializeField] Type type;
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
            // Decide whether starting or ending
            bool start = (type == Type.Start) ? true : false;

            onToggleCinematicBars.Raise(this, start);
        }
    }
}