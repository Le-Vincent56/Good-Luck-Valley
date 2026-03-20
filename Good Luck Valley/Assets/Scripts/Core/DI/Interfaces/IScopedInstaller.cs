using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Installer for additive scene containers (subscopes). Receives an <see cref="IScopeBuilder"/>
    /// which supports <see cref="IScopeBuilder.Import{T}"/> for pulling types from the parent scope.
    /// </summary>
    public interface IScopedInstaller
    {
        /// <summary>
        /// Registers all dependencies for an additive scene into the given builder.
        /// </summary>
        /// <param name="builder">The scope builder to register dependencies and imports with.</param>
        /// <param name="scene">The scene being installed, for use with RegisterFromScene.</param>
        void Install(IScopeBuilder builder, Scene scene);
    }
}