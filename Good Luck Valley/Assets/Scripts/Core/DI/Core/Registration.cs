using System;

namespace GoodLuckValley.Core.DI.Core
{
    /// <summary>
    /// Internal data structure representing a single type registration in the container.
    /// Maps an interface type to its implementation, lifetime, and cached instance.
    /// </summary>
    internal sealed class Registration
    {
        /// <summary>
        /// The type used as the resolution key (typically an interface).
        /// </summary>
        public Type InterfaceType { get; }
        
        /// <summary>
        /// The concrete type to instantiate. For instance registrations, this is the
        /// runtime type of the provided instance.
        /// </summary>
        public Type ImplementationType { get; }
        
        /// <summary>
        /// The lifetime management strategy for this registration.
        /// </summary>
        public Lifetime Lifetime { get; }
        
        /// <summary>
        /// Cached singleton instance, or the pre-provided instance for RegisterInstance calls.
        /// Set by the container on first resolution for singleton lifetime registrations.
        /// </summary>
        public object Instance { get; set; }
        
        /// <summary>
        /// True if this registration was created via RegisterInstance,
        /// (instance provided externally, not created by the container).
        /// </summary>
        public bool IsInstance { get; }
        
        /// <summary>
        /// True if this instance requires [Inject] field/property injection
        /// (e.g., MonoBehaviours registered via RegisterFromScene).
        /// </summary>
        public bool RequiresInjection { get; }

        /// <summary>
        /// Creates a type-mapped registration. The container will create instances via constructor
        /// resolution at resolve time.
        /// </summary>
        /// <param name="interfaceType">The resolution key (typically an interface type).</param>
        /// <param name="implementationType">The concrete type to instantiate.</param>
        /// <param name="lifetime">How the container manages instance lifetime.</param>
        public Registration(Type interfaceType, Type implementationType, Lifetime lifetime)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            IsInstance = false;
            RequiresInjection = false;
        }

        /// <summary>
        /// Creates an instance registration with a pre-provided object.
        /// Instances are always treated as singletons (the same object is returned every time).
        /// </summary>
        /// <param name="interfaceType">The resolution key (typically an interface type).</param>
        /// <param name="instance">The pre-created instance to return on resolution.</param>
        /// <param name="requiresInjection">True if the instance needs [Inject] field/property injection after container build.</param>
        public Registration(Type interfaceType, object instance, bool requiresInjection = false)
        {
            InterfaceType = interfaceType;
            ImplementationType = instance.GetType();
            Lifetime = Lifetime.Singleton;
            Instance = instance;
            IsInstance = true;
            RequiresInjection = requiresInjection;
        }
    }
}