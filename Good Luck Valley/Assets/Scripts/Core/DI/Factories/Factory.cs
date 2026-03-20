using GoodLuckValley.Core.DI.Interfaces;

namespace GoodLuckValley.Core.DI.Factories
{
    /// <summary>
    /// Container-backed factory for creating pure C# instances at runtime.
    /// Each call to <see cref="Create"/> resolves T from the container, producing
    /// a new instance for transient registrations or returning the cached singleton.
    /// </summary>
    /// <typeparam name="T">The type this factory creates.</typeparam>
    internal sealed class Factory<T> : IFactory<T> where T : class
    {
        private readonly IContainer _container;

        /// <summary>
        /// Creates a new factory backed by the given container.
        /// </summary>
        /// <param name="container">The container to resolve T from on each Create() call.</param>
        public Factory(IContainer container) => _container = container;

        /// <summary>
        /// Creates a new instance of T with all constructor dependencies resolved.
        /// </summary>
        /// <returns>A fully constructed and injected instance of T.</returns>
        public T Create() => _container.Resolve<T>();
    }
}