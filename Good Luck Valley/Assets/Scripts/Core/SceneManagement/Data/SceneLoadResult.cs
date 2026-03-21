using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.SceneManagement.Data
{
    /// <summary>
    /// Immutable result of a scene load operation.
    /// </summary>
    public readonly struct SceneLoadResult
    {
        /// <summary>
        /// Whether the scene loaded successfully.
        /// </summary>
        public bool Success { get; }
        
        /// <summary>
        /// The loaded scene. Only valid when <see cref="Success"/> is true.
        /// </summary>
        public Scene Scene { get; }
        
        /// <summary>
        /// Error message if the load failed. Null on success.
        /// </summary>
        public string ErrorMessage { get; }

        private SceneLoadResult(bool success, Scene scene, string errorMessage)
        {
            Success = success;
            Scene = scene;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a successful load result.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <returns>A SceneLoadResult representing a successful operation.</returns>
        public static SceneLoadResult Succeeded(Scene scene)
        {
            return new SceneLoadResult(true, scene, null);
        }

        /// <summary>
        /// Creates a failed load result with an error message.
        /// </summary>
        /// <param name="errorMessage">Description of the failure.</param>
        /// <returns>A SceneLoadResult representing a failed operation.</returns>
        public static SceneLoadResult Failed(string errorMessage)
        {
            return new SceneLoadResult(false, default, errorMessage);
        }
    }
}