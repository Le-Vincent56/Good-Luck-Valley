using System.Collections.Generic;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Lifecycle
{
    /// <summary>
    /// Static registry that tracks the application container and all active scene containers.
    /// Provides the public API for bootstrap code to register the application container
    /// and enqueue scene installations.
    /// </summary>
    public static class ContainerRegistry
    {
        private static IContainer _applicationContainer;
        private static readonly Dictionary<Scene, IContainer> _sceneContainers = new Dictionary<Scene, IContainer>();
        
        /// <summary>
        /// The application-scoped container that lives for the entire game session.
        /// Set during bootstrap via <see cref="SetApplicationContainer"/>.
        /// </summary>
        public static IContainer ApplicationContainer => _applicationContainer;
        
        /// <summary>
        /// All active scene containers. Used by the editor debug window.
        /// </summary>
        internal static IReadOnlyDictionary<Scene, IContainer> AllSceneContainers => _sceneContainers;

        /// <summary>
        /// Sets the application-scoped container. Called once during game bootstrap.
        /// </summary>
        /// <param name="container">The root application container.</param>
        public static void SetApplicationContainer(IContainer container) => _applicationContainer = container;

        /// <summary>
        /// Retrieves the container associated with the given scene, or null if none exists.
        /// </summary>
        /// <param name="scene">The scene for which to retrieve the container.</param>
        /// <returns>The container associated with the specified scene, or null if no container is found.</returns>
        public static IContainer GetContainerForScene(Scene scene)
        {
            _sceneContainers.TryGetValue(scene, out IContainer container);
            return container;
        }

        /// <summary>
        /// Enqueues a scoped scene installation to be processed in the next PlayerLoop
        /// injection phase. Use this for additive scenes that need a parent scope.
        /// </summary>
        /// <param name="scene">The scene to install.</param>
        /// <param name="installer">The root installer to configure the container.</param>
        public static void EnqueueSceneInstallation(Scene scene, IInstaller installer)
        {
            PlayerLoopInjection.Enqueue(new PendingInstallation(scene, installer));
        }

        /// <summary>
        /// Enqueues a scoped scene installation to be processed in the next PlayerLoop
        /// injection phase. Use this for additive scenes that need a parent scope.
        /// </summary>
        /// <param name="scene">The scene to install.</param>
        /// <param name="installer">The scoped installer with Import support.</param>
        /// <param name="parentScope">The parent container to import types from.</param>
        public static void EnqueueSceneInstallation(Scene scene, IScopedInstaller installer, IContainer parentScope)
        {
            PlayerLoopInjection.Enqueue(new PendingInstallation(scene, installer, parentScope));
        }

        /// <summary>
        /// Registers a built container for a scene. Called internally by
        /// <see cref="PlayerLoopInjection"/> after processing a pending installation.
        /// </summary>
        /// <param name="scene">The scene to associate with the specified container.</param>
        /// <param name="container">The container to register for the provided scene.</param>
        internal static void RegisterSceneContainer(Scene scene, IContainer container)
        {
            _sceneContainers[scene] = container;
        }

        /// <summary>
        /// Disposes and removes the container for the given scene.
        /// Called by <see cref="SceneLoader"/> before unloading a scene.
        /// </summary>
        /// <param name="scene"></param>
        internal static void UnregisterSceneContainer(Scene scene)
        {
            if (!_sceneContainers.TryGetValue(scene, out IContainer container)) return;
            
            container.Dispose();
            _sceneContainers.Remove(scene);
        }

        /// <summary>
        /// Disposes all containers and clears all state. Used for test
        /// cleanup and domain reload safety.
        /// </summary>
        internal static void Reset()
        {
            _applicationContainer?.Dispose();
            _applicationContainer = null;

            foreach (IContainer container in _sceneContainers.Values)                                                                                      {
                container.Dispose();
            }

            _sceneContainers.Clear();
        }
    }
}