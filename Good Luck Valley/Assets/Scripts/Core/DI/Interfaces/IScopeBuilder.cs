namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Extended builder for child scopes. Adds <see cref="Import{T}"/> to explicitly pull
    /// types from the parent container into this scope. Only imported types are accessible
    /// from the parent - there is no implicit resolution chain.
    /// </summary>
    public interface IScopeBuilder : IContainerBuilder
    {
        /// <summary>
        /// Imports a type from the parent scope, making it resolvable in this child scope.
        /// Throws at build time if the parent does not have a registration for hte type.
        /// </summary>
        /// <typeparam name="T">The type to import from the parent scope.</typeparam>
        void Import<T>();
    }
}