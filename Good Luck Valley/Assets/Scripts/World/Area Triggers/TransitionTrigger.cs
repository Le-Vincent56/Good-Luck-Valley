using GoodLuckValley.Entities;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    public class TransitionTrigger : MonoBehaviour
    {
        [SerializeField] private string sceneToTransitionTo;
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
            
        }

        private void Start()
        {
            Debug.Log(SceneToTransitionTo);
        }

        private void TriggerEnter(GameObject other)
        {
            Debug.Log(SceneToTransitionTo);
        }
    }
}