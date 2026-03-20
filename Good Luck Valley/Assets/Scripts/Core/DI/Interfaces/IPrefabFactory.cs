using UnityEngine;

namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Factory interface for instantiating prefab-based MonoBehaviours at runtime.
    /// Clones the registered prefab and injects all [Inject] fields before returning.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour type on the prefab.</typeparam>
    public interface IPrefabFactory<T> where T : MonoBehaviour
    {
        /// <summary>
        /// Instantiates the prefab at the given position and rotation, then injects dependencies.
        /// </summary>
        /// <param name="position">The world position for the instantiated object.</param>
        /// <param name="rotation">The rotation for the instantiated object.</param>
        /// <returns>The instantiated and injected MonoBehaviour.</returns>
        T Create(Vector3 position, Quaternion rotation);
        
        /// <summary>
        /// Instantiates the prefab at the given position, rotation, and parent, then injects dependencies.
        /// </summary>
        /// <param name="position">The world position for the instantiated object.</param>
        /// <param name="rotation">The rotation for the instantiated object.</param>
        /// <param name="parent">The parent transform for the instantiated object.</param>
        /// <returns>The instantiated and injected MonoBehaviour</returns>
        T Create(Vector3 position, Quaternion rotation, Transform parent);
    }
}