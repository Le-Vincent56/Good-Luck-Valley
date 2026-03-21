using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GoodLuckValley.Bootstrap
{
    /// <summary>
    /// Composition root for Good Luck Valley. Builds the application-scoped container
    /// at startup via [RuntimeInitializeOnLoadMethod]. No MonoBehaviour or scene object required.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Builds the application-scoped container before any scene loads.
        /// Application services persist for the entire game session.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // No-op - container build moved to OnInitialSceneLoaded so 
            // configuraiton asset can be loaded via Addressables
        }

        /// <summary>
        /// Installs the initial scene's container after Unity loads the first scene.
        /// Awake() has already fired; Start() fires next frame - injection slots in between.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void OnInitialSceneLoaded()
        {
            ContainerBuilder appBuilder = new ContainerBuilder("Application");
            
            // Load configuration assets from Addressables
            SceneRegistry sceneRegistry = await LoadAssetAsync<SceneRegistry>("SceneRegistry");
            LevelRegistry levelRegistry = await LoadAssetAsync<LevelRegistry>("LevelRegistry");

            if (!sceneRegistry)
            {
                Debug.LogError(
                    "[Bootstrapper] Failed to load SceneRegistry from Addressables. " +
                    "Scene management will not function."
                );
                
                return;
            }

            if (!levelRegistry)
            {
                Debug.LogError(
                    "[Bootstrapper] Failed to load LevelRegistry from Addressables. " +
                    "Level management will not function."
                );
                
                return;
            }
            
            IContainer appContainer = appBuilder.Build();
            ContainerRegistry.SetApplicationContainer(appContainer);
        }
        
        /// <summary>
        /// Loads an asset from Addressables by its address key.
        /// Returns null if the load fails.
        /// </summary>
        private static async Awaitable<T> LoadAssetAsync<T>(string address) where T : class
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);

            while (!handle.IsDone)
            {
                await Awaitable.NextFrameAsync();
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return handle.Result;

            Debug.LogError(
                $"[Bootstrapper] Failed to load Addressable asset '{address}': "
                + (handle.OperationException != null
                    ? handle.OperationException.Message
                    : "Unknown error")
            );
            
            return null;
        }
    }
}