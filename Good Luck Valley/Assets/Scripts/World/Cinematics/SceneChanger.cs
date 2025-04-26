using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;

namespace GoodLuckValley.World.Cinematics
{
    public class SceneChanger : MonoBehaviour
    {
        private enum SceneChangeType
        {
            System,
            Level
        }

        private SceneLoader sceneLoader;

        [Header("Fields")]
        [SerializeField] private SceneChangeType changeType;
        [SerializeField] private int sceneIndex;

        [Header("Level Change")]
        [SerializeField] private SceneGate toGate;
        [SerializeField] private bool showLoadingSymbol;

        private void Awake()
        {
            // Get the Scene Loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        /// <summary>
        /// Change the scene
        /// </summary>
        public void ChangeScene()
        {
            switch (changeType)
            {
                case SceneChangeType.System:
                    sceneLoader.ChangeSceneGroupSystem(sceneIndex);
                    break;

                case SceneChangeType.Level:
                    sceneLoader.LoadingFromGate = true;

                    sceneLoader.ChangeSceneGroupLevel(sceneIndex, toGate, showLoadingSymbol);
                    break;
            }
        }
    }
}
