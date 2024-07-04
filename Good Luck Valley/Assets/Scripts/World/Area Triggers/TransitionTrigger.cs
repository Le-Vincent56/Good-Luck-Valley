using GoodLuckValley.Entities;
using GoodLuckValley.SceneManagement;
using UnityEngine;

namespace GoodLuckValley.World.AreaTriggers
{
    public enum TransitionType
    {
        Entrance = 0,
        Exit = 1
    }

    [RequireComponent(typeof(AreaCollider))]
    public class TransitionTrigger : MonoBehaviour
    {
        [SerializeField] private TransitionType transitionType;
        [SerializeField] private int moveDirection;
        [SerializeField] private string sceneToTransitionTo;

        public TransitionType TransitionType
        {
            get => transitionType;
            set => transitionType = value;
        }

        public int MoveDirection
        {
            get => moveDirection;
            set => moveDirection = value;
        }

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
            // Don't trigger if loading already
            if (SceneLoader.Instance.IsLoading) return;

            SceneLoader.Instance.SetSceneToLoad(sceneToTransitionTo, transitionType, moveDirection);
            SceneLoader.Instance.BeginTransition();
        }
    }
}