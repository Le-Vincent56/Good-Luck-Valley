using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Injection;

namespace GoodLuckValley.Core.DI.Core
{
    /// <summary>
    /// Shared helper that manages type registrations and scene object tracking.
    /// Composed by both <see cref="ContainerBuilder"/> and <see cref="ScopeBuilder"/>
    /// to avoid duplicating registration logic.
    /// </summary>
    internal sealed class RegistrationCollection
    {
        private readonly Dictionary<Type, Registration> _registrations;
        private readonly List<SceneObjectEntry> _sceneObjects;

        public RegistrationCollection()
        {
            _registrations = new Dictionary<Type, Registration>();
            _sceneObjects = new List<SceneObjectEntry>();
        }

        /// <summary>
        /// Adds a type-mapped registration. The container will create instances
        /// via constructor injection at resolve time.
        /// </summary>
        /// <param name="interfaceType">The resolution key (typically an interface type).</param>
        /// <param name="implementationType">The concrete type to instantiate.</param>
        /// <param name="lifetime">How the container manages instance lifetime.</param>
        public void AddType(Type interfaceType, Type implementationType, Lifetime lifetime)
        {
            _registrations[interfaceType] = new Registration(interfaceType, implementationType, lifetime);
        }

        /// <summary>
        /// Adds a pre-existing instance registration. If requiresInjection is true,
        /// the instance is tracked for [Inject] field injection during the post-build pass.
        /// </summary>
        /// <param name="interfaceType">The resolution key (typically an interface type).</param>
        /// <param name="instance">The pre-created instance to return on resolution.</param>
        /// <param name="requiresInjection">
        /// True if the instance needs [Inject] field/property injection after container build
        /// (e.g., MonoBehaviours registered via RegisterFromScene).
        /// </param>
        public void AddInstance(Type interfaceType, object instance, bool requiresInjection = false)
        {
            _registrations[interfaceType] = new Registration(interfaceType, instance, requiresInjection);

            if (!requiresInjection) return;

            _sceneObjects.Add(new SceneObjectEntry(interfaceType, instance));
        }

        /// <summary>
        /// Returns a copy of the registrations dictionary for Container construction.
        /// A copy is returned so the Container owns its data independently of the builder.
        /// </summary>
        /// <returns>A new dictionary containing all registrations.</returns>
        public Dictionary<Type, Registration> CopyRegistrations()
        {
            return new Dictionary<Type, Registration>(_registrations);
        }
        
        /// <summary>
        /// Runs [Inject] field/property injection on all instances that were registered with requiresInjection: true
        /// (typically scene-discovered MonoBehaviours).
        /// Called after the container is fully built so all dependencies are resolvable.
        /// </summary>
        /// <param name="resolver">
        /// A function that resolves a <see cref="Type"/> to its registered instance
        /// (typically <c>container.Resolve</c>).
        /// </param>
        public void InjectSceneObjects(Func<Type, object> resolver)
        {
            foreach (SceneObjectEntry entry in _sceneObjects)
            {
                TypeInjectionInfo info = InjectionCache.GetOrCreate(entry.Type);

                if (!info.HasInjectables) continue;
                
                info.InjectInto(entry.Instance, resolver);
            }
        }

        /// <summary>
        /// Tracks a scene-discovered MonoBehaviour that needs [Inject] field injection
        /// after the container is built.
        /// </summary>
        private readonly struct SceneObjectEntry
        {
            public Type Type { get; }
            public object Instance { get; }

            public SceneObjectEntry(Type type, object instance)
            {
                Type = type;
                Instance = instance;
            }
        }
    }
}