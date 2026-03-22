using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.World.LevelManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Interfaces;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Bootstrap.Installers
{
    /// <summary>
    /// Scoped installer for level scenes. Creates a container scoped under the
    /// Gameplay Session Container. Imports ILevelManager so level adapters
    /// (triggers, markers) can access level orchestration.
    /// </summary>
    public class SampleLevelInstaller : IScopedInstaller
    {
        public void Install(IScopeBuilder builder, Scene scene)
        {
            // Import from Gameplay Session Container
            builder.Import<ILevelManager>();

            // Register level adapters that need [Inject] field injection
            builder.RegisterFromScene<LevelTransitionTrigger>(scene);

            // NOTE: SpawnPointMarker has no [Inject] fields — it is found at
            // runtime via SceneUtility.FindComponentInScene, not through DI.

            // TODO: RegisterFromScene only supports a single instance per type.
            // Levels with multiple LevelTransitionTriggers will need a
            // RegisterAllFromScene<T>() DI extension to inject all instances.
        }
    }
}