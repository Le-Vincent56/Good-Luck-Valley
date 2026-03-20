using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Interfaces
{
    /// <summary>
    /// Installer for root scene containers. Receives an <see cref="IContainerBuilder"/>
    /// to register scene-specific services and adapters.
    /// </summary>
    public interface IInstaller
    {
        /// <summary>
        /// Registers all dependencies for a root scene into the given builder.
        /// </summary>
        /// <param name="builder">The container builder to register dependencies with.</param>
        /// <param name="scene">The scene being installed, for use with RegisterFromScene.</param>
        void Install(IContainerBuilder builder, Scene scene);
    }
}