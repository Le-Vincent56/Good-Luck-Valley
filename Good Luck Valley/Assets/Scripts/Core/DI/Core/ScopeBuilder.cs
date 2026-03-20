using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Exceptions;
using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Core
{
    internal sealed class ScopeBuilder : IScopeBuilder
    {
        private readonly Container _parent;
        private readonly string _name;
        private readonly HashSet<Type> _imports;
        private readonly RegistrationCollection _registrations;

        internal ScopeBuilder(Container parent, string name = "Scope")
        {
            _parent = parent;
            _name = name;
            _imports = new HashSet<Type>();
            _registrations = new RegistrationCollection();
        }
        
        /// <summary>
        /// Imports a type from the parent scope, making it resolvable in this child scope.
        /// Throws at build time if the parent does not have a registration for the type.
        /// </summary>
        /// <typeparam name="T">The type to import from the parent scope.</typeparam>
        public void Import<T>()
        {
            Type type = typeof(T);

            if (!_parent.HasRegistration(type))
            {
                throw new InjectionException(
                    $"Cannot import {type.Name} into scope '{_name}': " +
                    $"no registration found in parent scope '{_parent.Name}'."
                );
            }

            _imports.Add(type);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers an <see cref="IPrefabFactory{T}"/> that instantiates the given prefab and injects [Inject]
        /// fields on the spawned instance.
        /// </summary>
        /// <param name="prefab">The prefab to clone when Create() is called.</param>
        /// <typeparam name="T">The MonoBehaviour type on the prefab.</typeparam>
        public void RegisterPrefabFactory<T>(T prefab) where T : MonoBehaviour
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds and returns the configured container. After this call, the builder
        /// should not be reused.
        /// </summary>
        /// <returns>The fully constructed and injected container.</returns>
        public IContainer Build()
        {
            Container container = new Container(
                _name,
                _registrations.CopyRegistrations(),
                _parent,
                new HashSet<Type>(_imports)
            );
            
            _parent.AddChild(container);
            _registrations.InjectSceneObjects(container.Resolve);

            return container;
        }
    }
}