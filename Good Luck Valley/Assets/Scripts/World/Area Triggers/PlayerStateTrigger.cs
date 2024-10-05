using GoodLuckValley.Player.Control;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    [RequireComponent(typeof(AreaCollider))]
    public class PlayerStateTrigger : MonoBehaviour
    {
        public enum Type
        {
            None = 0,
            Idle = 1,
            Slide = 2,
            Fall = 3
        }

        [SerializeField] private Type stateType;
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
            PlayerController player = other.GetComponent<PlayerController>();
            
            switch(stateType)
            {
                case Type.None:
                    player.TrySetState(0);
                    break;

                case Type.Idle:
                    player.TrySetState(1);
                    break;

                case Type.Slide:
                    player.TrySetState(2);
                    break;
            }
        }
    }
}