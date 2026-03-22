using System;
using System.Reflection;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;
using GoodLuckValley.Core.SceneManagement.Adapters;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Exceptions;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.SceneManagement.Services
{
    public sealed class SceneService : ISceneService
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly SceneRegistry _sceneRegistry;
        private readonly ITransitionService _transitionService;

        private SceneLoadState _loadState;
        
        /// <summary>
        /// The current state of the last scene load operation.
        /// </summary>
        public SceneLoadState LoadState => _loadState;
        
        /// <summary>
        /// True if a scene is currently being loaded or installed.
        /// </summary>
        public bool IsBusy => _loadState is SceneLoadState.Loading or SceneLoadState.Installing;

        /// <summary>
        /// Fired before a scene begins loading.
        /// </summary>
        public event Action<string> OnSceneLoadStarted;
        
        /// <summary>
        /// Fired after a scene is loaded and its container is installed.
        /// </summary>
        public event Action<string, Scene> OnSceneLoadCompleted;
        
        /// <summary>
        /// Fired when a scene load or container installation fails.
        /// </summary>
        public event Action<string, string> OnSceneLoadFailed;
        
        /// <summary>
        /// Fired after a scene is unloaded and its container is disposed.
        /// </summary>
        public event Action<Scene> OnSceneUnloaded;

        public SceneService(
            ISceneLoader sceneLoader,
            SceneRegistry sceneRegistry,
            ITransitionService transitionService
        )
        {
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
            _sceneRegistry = sceneRegistry ?? throw new ArgumentNullException(nameof(sceneRegistry));;
            _transitionService = transitionService ?? throw new ArgumentNullException(nameof(transitionService));;
        }

        /// <summary>
        /// Initializes the scene management system:
        /// 1. Loads the persistent transition scene (additive, no container).
        /// 2. Finds TransitionCanvasAdapter and wires it to ITransitionService.
        /// 3. Loads the initial scene (e.g., main menu) with a scoped container.
        /// </summary>
        /// <returns></returns>
        public async Awaitable InitializeAsync()
        {
            // Step 1: Load persistent transition scene
            string transitionSceneID = _sceneRegistry.TransitionSceneID;
            
            if(string.IsNullOrEmpty(transitionSceneID))
                throw new SceneManagementException("TransitionSceneID is not configured in SceneRegistry.");

            SceneEntry transitionEntry = _sceneRegistry.GetEntry(transitionSceneID);

            if (transitionEntry == null)
                throw new SceneManagementException($"Transition scene entry not found in SceneRegistry for ID '{transitionSceneID}'..");

            SceneLoadResult transitionResult = await _sceneLoader.LoadSceneAsync(
                transitionEntry.GetAddress(),
                SceneLoadMode.Additive
            );

            if (!transitionResult.Success)
                throw new SceneManagementException($"Failed to load transition scene: {transitionResult.ErrorMessage}");
            
            // Step 2: Find TransitioNCanvasAdapter and wire to ITransitionService
            TransitionCanvasAdapter adapter = Utilities.SceneUtility.FindComponentInScene<TransitionCanvasAdapter>(transitionResult.Scene);

            if (!adapter)
            {
                throw new SceneManagementException(
                    "TransitionCanvasAdapter not found in the persistent transition scene. " +
                    "Ensure the scene contains a GameObject with the " +
                    "TransitionCanvasAdapter component"
                );
            }
            
            _transitionService.SetCanvasAdapter(adapter);
            
            // Step 3: Load initial scene
            string initialSceneID = _sceneRegistry.InitialSceneID;

            if (string.IsNullOrEmpty(initialSceneID)) return;

            await LoadSceneAsync(initialSceneID, ContainerRegistry.ApplicationContainer);
        }

        /// <summary>
        /// Asynchronously loads a scene by its ID using Addressables, applies an optional DI container,
        /// and updates the scene load state with events for start, success, or failure.
        /// </summary>
        /// <param name="sceneID">The unique identifier of the scene to be loaded.</param>
        /// <param name="parentContainer">
        /// An optional dependency injection container that serves as the parent container for the scene.
        /// If null, no parent container is used.
        /// </param>
        /// <returns>
        /// A <see cref="SceneLoadResult"/> containing the result of the scene load operation,
        /// including success or failure information.
        /// </returns>
        public async Awaitable<SceneLoadResult> LoadSceneAsync(
            string sceneID,
            IContainer parentContainer = null
        )
        {
            if (IsBusy)
            {
                string error = $"Scene service is busy (state: {_loadState}. Cannot load {sceneID}.";
                OnSceneLoadFailed?.Invoke(sceneID, error);
                return SceneLoadResult.Failed(error);
            }

            SceneEntry entry = _sceneRegistry.GetEntry(sceneID);

            if (entry == null)
            {
                string error = "No SceneEntry found for scene ID '{sceneID}'.";
                OnSceneLoadFailed?.Invoke(sceneID, error);
                return SceneLoadResult.Failed(error);
            }

            _loadState = SceneLoadState.Loading;
            OnSceneLoadStarted?.Invoke(sceneID);
            
            // Load the scene via Addressables
            SceneLoadResult result = await _sceneLoader.LoadSceneAsync(
                entry.GetAddress(),
                SceneLoadMode.Additive
            );

            if (!result.Success)
            {
                _loadState = SceneLoadState.Error;
                OnSceneLoadFailed?.Invoke(sceneID, result.ErrorMessage);
                return result;;
            }
            
            // Install DI container if configured
            if (!entry.SkipContainerInstallation && !string.IsNullOrEmpty(entry.InstallerTypeName))
            {
                _loadState = SceneLoadState.Installing;

                try
                {
                    InstallContainer(result.Scene, entry, parentContainer);
                }
                catch (Exception ex)
                {
                    _loadState = SceneLoadState.Error;
                    string error = $"Container installation failed for '{sceneID}': {ex.Message}";
                    OnSceneLoadFailed?.Invoke(sceneID, error);
                    return SceneLoadResult.Failed(error);
                }
            }
            
            _loadState = SceneLoadState.Ready;
            OnSceneLoadCompleted?.Invoke(sceneID, result.Scene);
            return result;
        }

        /// <summary>
        /// Loads a scene additively without unloading the current active scene.
        /// This method uses the provided parent dependency container
        /// for resolving dependencies within the additively loaded scene.
        /// </summary>
        /// <param name="sceneID">The unique identifier of the scene to be loaded additively.</param>
        /// <param name="parentContainer">
        /// An optional parent dependency container used to provide scoped services
        /// to the additively loaded scene. If null, the default container is used.
        /// </param>
        /// <returns>The result of the scene load operation, encapsulated in a <see cref="SceneLoadResult"/> object,
        /// indicating success or failure with an optional error message.</returns>
        public async Awaitable<SceneLoadResult> LoadAdditiveSceneAsync(
            string sceneID,
            IContainer parentContainer
        )
        {
            return await LoadSceneAsync(sceneID, parentContainer);
        }

        /// <summary>
        /// Unloads a specified scene and cleans up its associated resources.
        /// 1. Unregisters the scene's container from the container registry.
        /// 2. Initiates asynchronous unloading of the scene.
        /// 3. Invokes the OnSceneUnloaded event upon successful completion.
        /// </summary>
        /// <param name="scene">The Unity scene to unload.</param>
        /// <returns>A task that resolves to true if the scene was successfully unloaded, otherwise false.</returns>
        public async Awaitable<bool> UnloadSceneAsync(Scene scene)
        {
            ContainerRegistry.UnregisterSceneContainer(scene);
            
            bool success = await _sceneLoader.UnloadSceneAsync(scene);
            
            if(success) OnSceneUnloaded?.Invoke(scene);

            return success;
        }

        /// <summary>
        /// Installs a dependency injection container for a specified scene.
        /// The method determines whether to use a scoped or a global installer
        /// based on the scene's configuration and initializes it accordingly.
        /// </summary>
        /// <param name="scene">The Unity scene for which the container is being installed.</param>
        /// <param name="entry">An object containing metadata about the scene
        /// and its installer configuration.</param>
        /// <param name="parentContainer">The parent container to be used when a scoped installer is required.
        /// This is mandatory if the scene specifies that a scoped container is to be used.</param>
        /// <exception cref="SceneManagementException">
        /// Thrown if the installer type cannot be resolved,
        /// if the installer cannot be instantiated, or if a scoped installer is required
        /// but no parent container is provided.
        /// </exception>
        private void InstallContainer(Scene scene, SceneEntry entry, IContainer parentContainer)
        {
            Type installerType = ResolveInstallerType(entry.InstallerTypeName);

            if (installerType == null)
            {
                throw new SceneManagementException(
                    $"Could not resolve installer type '{entry.InstallerTypeName}'. " +
                    "Ensure the type name is fully qualified (namespace and class) " +
                    "and the assembly containing it is loaded."
                );
            }

            if (entry.IsScoped)
            {
                if (parentContainer == null)
                {
                    throw new SceneManagementException(
                        $"Scene '{entry.SceneID}' requires a scoped installer " +
                        "but no parent container was provided."
                    );
                }

                IScopedInstaller scopedInstaller = (IScopedInstaller)Activator.CreateInstance(installerType);
                ContainerRegistry.InstallScene(scene, scopedInstaller, parentContainer);
            }
            else
            {
                IInstaller installer = (IInstaller)Activator.CreateInstance(installerType);
                ContainerRegistry.InstallScene(scene, installer);
            }
        }

        /// <summary>
        /// Resolves a type from its name within the currently loaded assemblies.
        /// Searches for a type by stripping the assembly qualifier from the provided type name and iterates
        /// through all assemblies loaded in the current application domain to locate the type.
        /// </summary>
        /// <param name="typeName">The fully qualified type name to resolve. This may optionally include an assembly qualifier, which will be ignored during the search.</param>
        /// <returns>The <see cref="Type"/> object of the resolved type if found; otherwise, <see langword="null"/> if the type could not be located.</returns>
        private static Type ResolveInstallerType(string typeName)
        {
            // Strip assembly qualifier if present — we search all assemblies directly
            int commaIndex = typeName.IndexOf(',');
            string nameOnly = commaIndex >= 0
                ? typeName.Substring(0, commaIndex).Trim()
                : typeName;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type type = assemblies[i].GetType(nameOnly);
                
                if (type == null) continue;
                
                return type;
            }

            return null;
        }
    }
}