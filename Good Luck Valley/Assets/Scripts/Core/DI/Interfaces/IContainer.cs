using System;

namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Read-only container interface for resolving dependencies. Consumers should depend
    /// on this interface rather than concrete implementation.
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// The name of this container, used for diagnostic messages and editor tooling.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Resolves a dependency by its registered type.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>The resolved instance.</returns>
        T Resolve<T>();
        
        /// <summary>
        /// Resolves a dependency by its registered type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The resolved instance.</returns>
        object Resolve(Type type);
        
        /// <summary>
        /// Attempts to resolve a dependency, returning false if the type is not registered.
        /// Does not catch circular dependency exceptions - those indicate bugs and should surface.
        /// </summary>
        /// <param name="instance">The resolved instance, or default if not found</param>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>True if the type was resolved successfully; false otherwise.</returns>
        bool TryResolve<T>(out T instance);
        
        /// <summary>
        /// Creates a new scope builder for constructing a child container with explicit imports.
        /// </summary>
        /// <param name="name">The name for the child scope, used for diagnostics and editor tooling.</param>
        /// <returns>A scope builder configured with this container as the parent.</returns>
        IScopeBuilder CreateScope(string name = "Scope");
    }
}