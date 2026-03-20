using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;

namespace GoodLuckValley.Core.DI.Factories
{
    /// <summary>
    /// Factory for instantiating prefab-based MonoBehaviours at runtime.
    /// Clones the registered prefab and injects all [Inject]-marked fields
    /// via <see cref="MonoBehaviourInjector"/> before returning the instance.
    /// </summary>
    /// <typeparam name="T">The MonoBehaviour type on the prefab.</typeparam>
    internal sealed class PrefabFactory<T> : IPrefabFactory<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly IContainer _container;

        public PrefabFactory(T prefab, IContainer container)
        {
            _prefab = prefab;
            _container = container;
        }

        /// <summary>
        /// Instantiates the prefab at the given position and rotation, then injects dependencies.
        /// </summary>
        /// <param name="position">The world position for the instantiated object.</param>
        /// <param name="rotation">The rotation for the instantiated object.</param>
        /// <returns>The instantiated and injected MonoBehaviour.</returns>
        public T Create(Vector3 position, Quaternion rotation)
        {
            T instance = Object.Instantiate(_prefab, position, rotation);
            MonoBehaviourInjector.InjectFields(instance, _container);
            return instance;
        }
        
        /// <summary>
        /// Instantiates the prefab at the given position, rotation, and parent, then injects dependencies.
        /// </summary>
        /// <param name="position">The world position for the instantiated object.</param>
        /// <param name="rotation">The rotation for the instantiated object.</param>
        /// <param name="parent">The parent transform for the instantiated object.</param>
        /// <returns>The instantiated and injected MonoBehaviour</returns>
        public T Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            T instance = Object.Instantiate(_prefab, position, rotation, parent);
            MonoBehaviourInjector.InjectFields(instance, _container);
            return instance;
        }
    }
}