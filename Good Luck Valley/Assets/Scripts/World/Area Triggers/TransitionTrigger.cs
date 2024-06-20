using GoodLuckValley.Entities;
using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    public class TransitionTrigger : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onTransition;

        private string sceneToTransitionTo;
        public string SceneToTransitionTo
        {
            get => sceneToTransitionTo;
            set => sceneToTransitionTo = value;
        }

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
            onTransition.Raise(this, null);
        }
    }
}