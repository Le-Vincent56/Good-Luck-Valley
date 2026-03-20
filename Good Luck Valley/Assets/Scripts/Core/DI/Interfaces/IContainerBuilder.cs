using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Builder interface for configuring and constructing a root dependency container.
    /// Registration methods return void - the builder pattern is sequential, not fluent.
    /// </summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// Registers a singleton mapping from interface to implementation.
        /// The container creates one instance via constructor injection and caches it.
        /// </summary>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        /// <typeparam name="TImplementation">The concrete type to instantiate.</typeparam>
        void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface;

        /// <summary>
        /// Registers a concrete type of its own singleton.
        /// </summary>
        /// <typeparam name="T">The concrete type to register and resolve.</typeparam>
        void RegisterSingleton<T>() where T : class;
        
        /// <summary>
        /// Registers a transient mapping from interface to implementation.
        /// A new instance is created via constructor injection for each resolution.
        /// </summary>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        /// <typeparam name="TImplementation">The concrete type to instantiate.</typeparam>
        void RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface;
        
        /// <summary>
        /// Registers a concrete type as its own transient.
        /// </summary>
        /// <typeparam name="T">The concrete type to register and resolve.</typeparam>
        void RegisterTransient<T>() where T : class;

        /// <summary>
        /// Registers a pre-existing instance as a singleton for the given interface type.
        /// The same instance is returned for every resolution.
        /// </summary>
        /// <param name="instance">The pre-created instance to return on resolution.</param>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        void RegisterInstance<TInterface>(TInterface instance);

        /// <summary>
        /// Finds a MonoBehaviour of type T in the given scene and registers it.
        /// The found component will have its [Inject] fields populated after Build().
        /// </summary>
        /// <param name="scene">The scene to search for the component.</param>
        /// <typeparam name="T">The MonoBehaviour type to find and register.</typeparam>
        void RegisterFromScene<T>(Scene scene) where T : MonoBehaviour;

        /// <summary>
        /// Registers an <see cref="IFactory{T}"/> backed by the container for runtime creation
        /// of instances via constructor injection.
        /// </summary>
        /// <typeparam name="T">The type the factory will create.</typeparam>
        void RegisterFactory<T>() where T : class;
        
        /// <summary>
        /// Registers an <see cref="IPrefabFactory{T}"/> that instantiates the given prefab and injects [Inject]
        /// fields on the spawned instance.
        /// </summary>
        /// <param name="prefab">The prefab to clone when Create() is called.</param>
        /// <typeparam name="T">The MonoBehaviour type on the prefab.</typeparam>
        void RegisterPrefabFactory<T>(T prefab) where T : MonoBehaviour;

        /// <summary>
        /// Builds and returns the configured container. After this call, the builder
        /// should not be reused.
        /// </summary>
        /// <returns>The fully constructed and injected container.</returns>
        IContainer Build();
    }
}