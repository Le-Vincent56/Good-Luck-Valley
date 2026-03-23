using System;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.SceneManagement.Interfaces
{
    /// <summary>
    /// Primary scene management API. Orchestrates Addressables scene loading,
    /// DI container lifecycle (install on load, dispose on unload), and installer
    /// resolution from <see cref="SceneRegistry"/>.
    /// </summary>
    public interface ISceneService
    {
        /// <summary>
        /// The current state of the last scene load operation.
        /// </summary>
        SceneLoadState LoadState { get; }
        
        /// <summary>
        /// True if a scene is currently being loaded or installed.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Fired before a scene begins loading.
        /// </summary>
        event Action<string> OnSceneLoadStarted;
        
        /// <summary>
        /// Fired after a scene is loaded and its container is installed.
        /// </summary>
        event Action<string, Scene> OnSceneLoadCompleted;

        /// <summary>
        /// Fired when a scene load or container installation fails.
        /// </summary>
        event Action<string, string> OnSceneLoadFailed;

        /// <summary>
        /// Fired after a scene is unloaded and its container is disposed.
        /// </summary>
        event Action<Scene> OnSceneUnloaded;

        /// <summary>
        /// Initializes the scene management system. Loads the persistent transition
        /// scene, wires the TransitionCanvasAdapter, and navigates to the initial scene.
        /// </summary>
        /// <returns>
        /// An awaitable task that completes when the initialization process finishes.
        /// </returns>
        Awaitable InitializeAsync();

        /// <summary>
        /// Loads a scene by its ID from the SceneRegistry. If a parent container is provided
        /// and the scene entry is scoped, a child container is created. If no parent is provided
        /// and the entry is not scoped, a root container is created.
        /// </summary>
        /// <param name="sceneID">The scene ID as registered in SceneRegistry.</param>
        /// <param name="parentContainer">An optional parent container for scoped installations.</param>
        /// <returns>The result of the load operation.</returns>
        Awaitable<SceneLoadResult> LoadSceneAsync(string sceneID, IContainer parentContainer = null);
        
        /// <summary>
        /// Loads a scene by its stable ID from the SceneRegistry using O(1) dictionary lookup.
        /// </summary>
        /// <param name="stableID">The stable ID as baked into the SceneEntry.</param>
        /// <param name="parentContainer">An optional parent container for scoped installations.</param>
        /// <returns>The result of the load operation.</returns>
        Awaitable<SceneLoadResult> LoadSceneAsync(int stableID, IContainer parentContainer = null);
        
        /// <summary>
        /// Unloads a scene, disposing its DI container first via ContainerRegistry.
        /// </summary>
        /// <param name="scene">The scene to unload.</param>
        /// <returns>True if the scene was unloaded successfully.</returns>
        Awaitable<bool> UnloadSceneAsync(Scene scene);
        
        /// <summary>
        /// Convenience method for loading a scene additively with a parent container.
        /// Equivalent to calling SceneLoadAsync with a parent container.
        /// </summary>
        /// <param name="sceneID">The scene ID as registered in SceneRegistry.</param>
        /// <param name="parentContainer">The parent container for scoped installation.</param>
        /// <returns>The result of the load operation.</returns>
        Awaitable<SceneLoadResult> LoadAdditiveSceneAsync(string sceneID, IContainer parentContainer);
        
        /// <summary>
        /// Convenience method for loading a scene additively by stable ID with a parent container.
        /// </summary>
        /// <param name="stableID">The stable ID as baked into the SceneEntry.</param>
        /// <param name="parentContainer">The parent container for scoped installation.</param>
        /// <returns>The result of the load operation.</returns>
        Awaitable<SceneLoadResult> LoadAdditiveSceneAsync(int stableID, IContainer parentContainer);
    }
}