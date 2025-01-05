using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTransporter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneGroupData sceneGroupData;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField, HideInInspector] private int sceneIndexToLoad;

        public int SceneIndexToLoad { get => sceneIndexToLoad; set => sceneIndexToLoad = value; }

        public SceneGroupData SceneGroupData { get => sceneGroupData; }

        private void Start()
        {
            // Retrieve the SceneLoader service
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            sceneLoader.ChangeSceneGroup(sceneIndexToLoad);
        }
    }
}
