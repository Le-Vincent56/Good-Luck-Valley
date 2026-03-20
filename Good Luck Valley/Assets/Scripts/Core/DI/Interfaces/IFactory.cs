namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Factory interface for creating pure C# instances at runtime via the container.
    /// Gameplay code depends on this interface rather than resolving from the container directly.
    /// </summary>
    /// <typeparam name="T">The type this factory creates.</typeparam>
    public interface IFactory<out T> where T : class
    {
        /// <summary>
        /// Creates a new instance of T with all constructor dependencies resolved.
        /// </summary>
        /// <returns>A fully constructed and injected instance of T.</returns>
        T Create();
    }
}