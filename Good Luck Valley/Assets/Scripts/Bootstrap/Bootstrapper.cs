using System;
using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;
using GoodLuckValley.Core.Input.Adapters;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.Input.Services;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using GoodLuckValley.Core.SceneManagement.Services;
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
        /// Reserved for pre-Addressables setup that must happen before the
        /// initial scene's Awake() calls
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // No-op - container build moved to OnInitialSceneLoaded so 
            // configuraiton asset can be loaded via Addressables
        }

        /// <summary>
        /// Called after Unity loads the initial scene. Loads configuration assets
        /// via Addressables, builds the application container, and kicks off the scene
        /// management initialization sequence
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void OnInitialSceneLoaded()
        {
            try
            {
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

                // Build the application-scoped container
                ContainerBuilder appBuilder = new ContainerBuilder("Application");

                appBuilder.RegisterInstance<SceneRegistry>(sceneRegistry);
                appBuilder.RegisterInstance<LevelRegistry>(levelRegistry);
                appBuilder.RegisterSingleton<ISceneLoader, AddressablesSceneLoader>();
                appBuilder.RegisterSingleton<ITransitionService, TransitionService>();
                appBuilder.RegisterSingleton<ISceneService, SceneService>();
                
                // Input
                InputService inputService = new InputService(() => Time.time);
                appBuilder.RegisterInstance<IPlayerInput>(inputService);
                appBuilder.RegisterInstance<IUIInput>(inputService);
                appBuilder.RegisterInstance<IInputContextService>(inputService);
                
                // Create persistent InputAdapter GameObject and wire to InputService
                GameObject inputAdapterGo = new GameObject("InputAdapter");
                UnityEngine.Object.DontDestroyOnLoad(inputAdapterGo);
                InputAdapter inputAdapter = inputAdapterGo.AddComponent<InputAdapter>();
                inputAdapter.Initialize(inputService);
                
                IContainer appContainer = appBuilder.Build();
                ContainerRegistry.SetApplicationContainer(appContainer);

                // Iniitalize scene management (loads transition scene, then initial scene)
                ISceneService sceneService = appContainer.Resolve<ISceneService>();
                await sceneService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("[Bootstrapper] Initialization failed: " + ex.Message);
                Debug.LogException(ex);
            }
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