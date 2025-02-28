using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class SceneChanger : MonoBehaviour
    {
        private SceneLoader sceneLoader;
        [SerializeField] private int sceneIndex;

        private void Awake()
        {
            // Get the Scene Loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        /// <summary>
        /// Change the scene
        /// </summary>
        public void ChangeScene() => sceneLoader.ChangeSceneGroupSystem(sceneIndex);
    }
}
