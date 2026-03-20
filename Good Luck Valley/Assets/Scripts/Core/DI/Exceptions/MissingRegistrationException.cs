using System;

namespace GoodLuckValley.Core.DI.Exceptions
{
    /// <summary>
    /// Thrown when attempting to resolve a type that has no registration in the container
    /// and is not imported from a parent scope.
    /// </summary>
    public sealed class MissingRegistrationException : InjectionException
    {
        /// <summary>
        /// The type that could not be resolved.
        /// </summary>
        public Type RequestedType { get; }

        /// <summary>
        /// The name of the container or scope where resolution was attempted.
        /// </summary>
        public string ContainerName { get; }

        public MissingRegistrationException(Type requestedType, string containerName)
            : base($"No registration found for {requestedType.Name} in scope '{containerName}'. " +
                   $"Did you forget to register it or import it from a parent scope?")
        {
            RequestedType = requestedType;
            ContainerName = containerName;
        }
    }
}