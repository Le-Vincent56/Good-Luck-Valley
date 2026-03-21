using System.Collections.Generic;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.SceneManagement.Services
{
    /// <summary>
    /// <see cref="ISceneLoader"/> implementation that wraps Unity Addressables for
    /// scene loading and unloading. Tracks Scene-to-SceneInstance mappings internally
    /// for proper Addressables handle release on unload.
    /// </summary>
    public sealed class AddressablesSceneLoader : ISceneLoader
    {
        private readonly Dictionary<Scene, SceneInstance> _sceneInstances = new Dictionary<Scene, SceneInstance>();

        /// <summary>
        /// Loads a scene via Addressbales by its address key.
        /// </summary>
        /// <param name="address">The address of the scene to load using Addressables.</param>
        /// <param name="loadMode">The mode to load the scene, either Replace or Additive.</param>
        /// <param name="activateOnLoad">Indicates whether the scene should be activated immediately after loading. Default is true.</param>
        /// <returns>A result wrapping the success status of the operation, the loaded scene if successful, or an error message if failed.</returns>
        public async Awaitable<SceneLoadResult> LoadSceneAsync(
            string address,
            SceneLoadMode loadMode,
            bool activateOnLoad = true
        )
        {
            LoadSceneMode unityLoadMode = loadMode == SceneLoadMode.Additive
                ? LoadSceneMode.Additive
                : LoadSceneMode.Single;
            
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(
                address, 
                unityLoadMode, 
                activateOnLoad
            );

            while (!handle.IsDone)
            {
                await Awaitable.NextFrameAsync();
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SceneInstance sceneInstance = handle.Result;
                _sceneInstances[sceneInstance.Scene] = sceneInstance;
                return SceneLoadResult.Succeeded(sceneInstance.Scene);
            }

            string error = handle.OperationException != null
                ? handle.OperationException.Message
                : $"Unknown error loading scene at address '{address}'.";

            return SceneLoadResult.Failed(error);
        }

        /// <summary>
        /// Unloads a scene previously loaded through this loader. Releases the
        /// Addressables handle to free the asset bundle.
        /// </summary>
        /// <param name="scene">The Unity scene to unload.</param>
        /// <returns>True if the scene was successfully unloaded, false otherwise.</returns>
        public async Awaitable<bool> UnloadSceneAsync(Scene scene)
        {
            if (!_sceneInstances.TryGetValue(scene, out SceneInstance sceneInstance))
                return false;
            
            AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(sceneInstance);

            while (!handle.IsDone)
            {
                await Awaitable.NextFrameAsync();
            }

            _sceneInstances.Remove(scene);
            return handle.Status == AsyncOperationStatus.Succeeded;
        }
    }
}