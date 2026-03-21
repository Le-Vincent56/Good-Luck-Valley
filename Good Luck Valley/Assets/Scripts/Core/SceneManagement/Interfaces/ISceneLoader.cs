using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.SceneManagement.Interfaces
{
    /// <summary>
    /// Abstraction over scene loading mechanics. The default implementation wraps Unity Addressables.
    /// Can be substituted with a mock for testing.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads a scene asynchronously by its Addressable address.
        /// </summary>
        /// <param name="address">The Addressable address or key of the scene.</param>
        /// <param name="loadMode">Whether to load as Replace or Additive.</param>
        /// <param name="activateOnLoad">Whether to activate the scene immediately after loading.</param>
        /// <returns>The result of the load operation.</returns>
        Awaitable<SceneLoadResult> LoadSceneAsync(
            string address,
            SceneLoadMode loadMode,
            bool activateOnLoad = true
        );

        /// <summary>
        /// Unloads a previously loaded scene.
        /// </summary>
        /// <param name="scene">The scene to unload.</param>
        /// <returns>True if the scene was unloaded successfully</returns>
        Awaitable<bool> UnloadSceneAsync(Scene scene);
    }
}