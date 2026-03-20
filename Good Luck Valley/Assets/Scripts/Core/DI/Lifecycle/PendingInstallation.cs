using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Lifecycle
{
    /// <summary>
    /// Internal data class representing a scene installation waiting to be processed
    /// by the PlayerLoop injection phase. Either <see cref="Installer"/> or
    /// <see cref="ScopedInstaller"/> will be set, never both.
    /// </summary>
    internal sealed class PendingInstallation
    {
        /// <summary>
        /// The scene whose MonoBehaviours will be discovered and injected.
        /// </summary>
        public Scene Scene { get; }
        
        /// <summary>
        /// The installer for root scene containers. Null for scoped installations.
        /// </summary>
        public IInstaller Installer { get; }
        
        /// <summary>
        /// The installer for additive scene containers. Null for root installations.
        /// </summary>
        public IScopedInstaller ScopedInstaller { get; }
        
        /// <summary>
        /// The parent container for scoped installations. Null for root installations.
        /// </summary>
        public IContainer ParentScope { get; }

        public PendingInstallation(Scene scene, IInstaller installer)
        {
            Scene = scene;
            Installer = installer;
        }

        public PendingInstallation(Scene scene, IScopedInstaller scopedInstaller, IContainer parentScope)
        {
            Scene = scene;
            ScopedInstaller = scopedInstaller;
            ParentScope = parentScope;
        }
    }
}