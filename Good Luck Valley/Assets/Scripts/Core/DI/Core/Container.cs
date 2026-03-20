using System;
using System.Collections.Generic;
using System.Reflection;
using GoodLuckValley.Core.DI.Exceptions;
using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;

namespace GoodLuckValley.Core.DI.Core
{
    /// <summary>
    /// A unified contianer implementation for both root and scoped containers.
    /// Root containers have no parent; scoped containers have a parent and an explicit import set.
    /// Only types registered locally or explicitly imported from the parent are resolvable.
    /// </summary>
    internal sealed class Container : IContainer
    {
        private readonly string _name;
        private readonly Dictionary<Type, Registration> _registrations;
        private readonly Container _parent;
        private readonly HashSet<Type> _imports;
        private readonly List<Container> _children;
        private readonly List<IDisposable> _disposables;
        private bool _disposed;

        /// <summary>
        /// The name of this container, used for diagnostic messages and editor tooling.
        /// </summary>
        public string Name => _name;
        
        internal IReadOnlyDictionary<Type, Registration> Registrations => _registrations;
        internal IReadOnlyCollection<Type> Imports => _imports;
        internal IReadOnlyList<Container> Children => _children;
        internal Container Parent => _parent;

        internal Container(
            string name,
            Dictionary<Type, Registration> registrations,
            Container parent = null,
            HashSet<Type> imports = null
        )
        {
            _name = name;
            _registrations = registrations;
            _parent = parent;
            _imports = imports;
            _children = new List<Container>();
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Resolves a dependency by its registered type.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The resolved instance.</returns>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Resolves a dependency by its registered type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance.</returns>
        public object Resolve(Type type)
        {
            ThrowIfDisposed();
            return ResolveInternal(type, new List<Type>());
        }

        /// <summary>
        /// Attempts to resolve a dependency, returning false if the type is not registered.
        /// Does not catch circular dependency exceptions - those indicate bugs and should surface.
        /// </summary>
        /// <param name="instance">The resolved instance, or default if not found.</param>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>True if the type was resolved successfully; false otherwise.</returns>
        public bool TryResolve<T>(out T instance)
        {
            try
            {
                instance = Resolve<T>();
                return true;
            }
            catch (MissingRegistrationException)
            {
                instance = default;
                return false;
            }
        }

        /// <summary>
        /// Creates a new scope builder for constructing a child container with explicit imports.
        /// </summary>
        /// <param name="name">The name for the child scope, used for diagnostics and editor tooling.</param>
        /// <returns>A scope builder configured with this container as the parent.</returns>
        public IScopeBuilder CreateScope(string name = "Scope")
        {
            ThrowIfDisposed();

            return new ScopeBuilder(this, name);
        }

        /// <summary>
        /// Resolves an instance of the specified type within the current container or its parent container,
        /// checking for registrations and imported types.
        /// </summary>
        /// <param name="type">The type to resolve, which should have a corresponding registration.</param>
        /// <param name="resolutionChain">The chain of types being resolved to track circular dependencies during resolution.</param>
        /// <returns>The resolved object instance for the specified type.</returns>
        /// <exception cref="MissingRegistrationException">Thrown when no registration or import is found for the specified type.</exception>
        private object ResolveInternal(Type type, List<Type> resolutionChain)
        {
            // Check local registrations first
            if (_registrations.TryGetValue(type, out Registration registration))
                return ResolveRegistration(registration, resolutionChain);

            if (_parent != null && _imports != null && _imports.Contains(type))
                return _parent.ResolveInternal(type, resolutionChain);

            throw new MissingRegistrationException(type, _name);
        }

        /// <summary>
        /// Resolves a registration by creating or retrieving an instance of the object
        /// based on the registration's lifetime and configuration.
        /// </summary>
        /// <param name="registration">The registration containing details about the type, its lifetime, and its current instance if applicable.</param>
        /// <param name="resolutionChain">The chain of types currently being resolved to detect circular dependencies.</param>
        /// <returns>Returns the resolved object instance, either newly created or reused based on the lifetime.</returns>
        /// <exception cref="InjectionException">Thrown when an invalid lifetime configuration is encountered.</exception>
        private object ResolveRegistration(Registration registration, List<Type> resolutionChain)
        {
            // Instance registrations already have their value
            if (registration.IsInstance) return registration.Instance;

            switch (registration.Lifetime)
            {
                case Lifetime.Singleton:
                    if (registration.Instance != null) return registration.Instance;

                    object singletonInstance = CreateInstance(registration, resolutionChain);
                    registration.Instance = singletonInstance;
                    return singletonInstance;

                case Lifetime.Transient:
                    return CreateInstance(registration, resolutionChain);

                default:
                    throw new InjectionException(
                        $"Unknown lifetime '{registration.Lifetime}' for type " +
                        $"'{registration.InterfaceType.Name}'"
                    );
            }
        }

        /// <summary>
        /// Creates an instance of the specified type defined in the registration
        /// and resolves its dependencies using the given resolution chain.
        /// </summary>
        /// <param name="registration">The registration containing information about the type to instantiate.</param>
        /// <param name="resolutionChain">The chain of types being resolved to detect circular dependencies.</param>
        /// <returns>Returns the created object instance with its dependencies resolved.</returns>
        /// <exception cref="CircularDependencyException">Thrown when a circular dependency is detected during resolution.</exception>
        private object CreateInstance(Registration registration, List<Type> resolutionChain)
        {
            Type type = registration.ImplementationType;

            // Circular dependency check
            if (resolutionChain.Contains(type))
            {
                throw new CircularDependencyException(resolutionChain, type);
            }

            resolutionChain.Add(type);
            ConstructorInfo constructor = InjectionCache.GetConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = ResolveInternal(parameters[i].ParameterType, resolutionChain);
            }

            resolutionChain.Remove(type);

            object instance = constructor.Invoke(args);

            // Track IDisposable singletons for cascading disposal
            if (registration.Lifetime == Lifetime.Singleton && instance is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }

            return instance;
        }

        /// <summary>
        /// Adds a child container to the list of tracked child containers.
        /// </summary>
        /// <param name="child">The child container to be added.</param>
        internal void AddChild(Container child) => _children.Add(child);

        /// <summary>
        /// Removes a child container from the list of tracked child containers.
        /// </summary>
        /// <param name="child">The child container to be removed.</param>
        internal void RemoveChild(Container child) => _children.Remove(child);

        /// <summary>
        /// Adds a registration after the container has been constructed.
        /// Used exclusively during the build process for factory registrations that
        /// require a container reference (which doesn't exist at registration time).
        /// </summary>
        /// <param name="type">The resolution key type.</param>
        /// <param name="registration">The registration to add.</param>
        internal void AddPostBuildRegistration(Type type, Registration registration)
        {
            _registrations[type] = registration;
        }

        /// <summary>
        /// Checks whether ths given type can be resolved from this container,
        /// either the local registrations or through the import chain to parent scopes.
        /// Does not create instances - used by ScopeBuilder to validate imports at build time.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if the type is resolvable from this container.</returns>
        internal bool HasRegistration(Type type)
        {
            if (_registrations.ContainsKey(type)) return true;
            
            if(_parent != null && _imports != null && _imports.Contains(type)) return _parent.HasRegistration(type);

            return false;
        }
        
        /// <summary>
        /// Releases the resources used by this container instance.
        /// </summary>
        /// <remarks>
        /// The method disposes of all nested child containers, the singletons
        /// registered within the container, and clears internal state to free resources.
        /// It ensures that resources are released in proper order:
        /// 1. Disposes child containers recursively.
        /// 2. Disposes internally tracked disposable objects in reverse registration order.
        /// 3. Detaches this container from its parent, if applicable.
        /// 4. Clears all collections and state within the container.
        /// Subsequent calls to this method will have no effect if the container has already been disposed.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if an attempt is made to use the container after it has been disposed.
        /// </exception>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            
            // 1. Dispose children first (cascading) — copy to avoid modification during iteration
            List<Container> childrenCopy = new List<Container>(_children);
            foreach (Container child in childrenCopy)
            {
                child.Dispose();
            }

            // 2. Dispose owned singletons in reverse registration order
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i].Dispose();
            }

            // 3. Remove self from the parent's child list
            _parent?.RemoveChild(this);

            // 4. Clear state
            _registrations.Clear();
            _children.Clear();
            _disposables.Clear();
        }

        /// <summary>
        /// Ensures the container has not been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the container is accessed after it has been disposed.
        /// The exception message includes the name of the disposed container for
        /// diagnostic purposes.
        /// </exception>
        private void ThrowIfDisposed()
        {
            if (!_disposed) return;

            throw new ObjectDisposedException(_name, $"Cannot access disposed container '{_name}'.");
        }
    }
}