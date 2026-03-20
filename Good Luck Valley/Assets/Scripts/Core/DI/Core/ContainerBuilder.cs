using System;
using GoodLuckValley.Core.DI.Factories;
using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Core
{
    /// <summary>
    /// Builder for constructing root containers. Collects registrations via the
    /// <see cref="IContainerBuilder"/> interface and produces a <see cref="IContainer"/>
    /// on <see cref="Build"/>. After Build() is called, the builder should not be reused.
    /// </summary>
    public sealed class ContainerBuilder : IContainerBuilder
    {
        private readonly string _name;
        private readonly RegistrationCollection _registrations;

        public ContainerBuilder(string name = "Container")
        {
            _name = name;
            _registrations = new RegistrationCollection();
        }

        /// <summary>
        /// Registers a singleton mapping from interface to implementation.
        /// The container creates one instance via constructor injection and caches it.
        /// </summary>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        /// <typeparam name="TImplementation">The concrete type to instantiate.</typeparam>
        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            _registrations.AddType(typeof(TInterface), typeof(TImplementation), Lifetime.Singleton);
        }

        /// <summary>
        /// Registers a concrete type of its own singleton.
        /// </summary>
        /// <typeparam name="T">The concrete type to register and resolve.</typeparam>
        public void RegisterSingleton<T>() where T : class
        {
            _registrations.AddType(typeof(T), typeof(T), Lifetime.Singleton);
        }

        /// <summary>
        /// Registers a transient mapping from interface to implementation.
        /// A new instance is created via constructor injection for each resolution.
        /// </summary>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        /// <typeparam name="TImplementation">The concrete type to instantiate.</typeparam>
        public void RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            _registrations.AddType(typeof(TInterface), typeof(TImplementation), Lifetime.Transient);
        }

        /// <summary>
        /// Registers a concrete type as its own transient.
        /// </summary>
        /// <typeparam name="T">The concrete type to register and resolve.</typeparam>
        public void RegisterTransient<T>() where T : class
        {
            _registrations.AddType(typeof(T), typeof(T), Lifetime.Transient);
        }

        /// <summary>
        /// Registers a pre-existing instance as a singleton for the given interface type.
        /// The same instance is returned for every resolution.
        /// </summary>
        /// <param name="instance">The pre-created instance to return on resolution.</param>
        /// <typeparam name="TInterface">The type used as the resolution key.</typeparam>
        public void RegisterInstance<TInterface>(TInterface instance)
        {
            _registrations.AddInstance(typeof(TInterface), instance);
        }

        /// <summary>
        /// Finds a MonoBehaviour of type T in the given scene and registers it.
        /// The found component will have its [Inject] fields populated after Build().
        /// </summary>
        /// <param name="scene">The scene to search for the component.</param>
        /// <typeparam name="T">The MonoBehaviour type to find and register.</typeparam>
        public void RegisterFromScene<T>(Scene scene) where T : MonoBehaviour
        {
            T component = MonoBehaviourInjector.FindInScene<T>(scene);
            _registrations.AddInstance(typeof(T), component, true);
        }

        /// <summary>
        /// Registers an <see cref="IFactory{T}"/> backed by the container for runtime creation
        /// of instances via constructor injection.
        /// </summary>
        /// <typeparam name="T">The type the factory will create.</typeparam>
        public void RegisterFactory<T>() where T : class
        {
            _registrations.AddPostBuildAction(container =>
            {
                Factory<T> factory = new Factory<T>(container);
                container.AddPostBuildRegistration(
                    typeof(IFactory<T>),
                    new Registration(typeof(IFactory<T>), factory)
                );
            });
        }

        /// <summary>
        /// Registers an <see cref="IPrefabFactory{T}"/> that instantiates the given prefab and injects [Inject]
        /// fields on the spawned instance.
        /// </summary>
        /// <param name="prefab">The MonoBehaviour type on the prefab.</param>
        /// <typeparam name="T">The prefab to clone when Create() is called.</typeparam>
        public void RegisterPrefabFactory<T>(T prefab) where T : MonoBehaviour
        {
            _registrations.AddPostBuildAction(container =>
            {
                PrefabFactory<T> prefabFactory = new PrefabFactory<T>(prefab, container);
                container.AddPostBuildRegistration(
                    typeof(IPrefabFactory<T>),
                    new Registration(typeof(IPrefabFactory<T>), prefabFactory)
                );
            });
        }

        /// <summary>
        /// Builds and returns the configured container. After this call, the builder
        /// should not be reused.
        /// </summary>
        /// <returns>The fully constructed and injected container.</returns>
        public IContainer Build()
        {
            Container container = new Container(_name, _registrations.CopyRegistrations());
            _registrations.RunPostBuildActions(container);
            _registrations.InjectSceneObjects(container.Resolve);
            
            return container;
        }
    }
}