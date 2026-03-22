using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using GoodLuckValley.World.LevelManagement.Data;
using GoodLuckValley.World.LevelManagement.Interfaces;
using GoodLuckValley.World.LevelManagement.Services;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Bootstrap.Installers
{
    /// <summary>
    /// Scoped installer for the Persistent Gameplay Scene. Creates a container
    /// scoped under the Application Container that lives for the entire gameplay
    /// session. Level containers are scoped children of this container.
    /// </summary>
    public class GameplaySessionInstaller : IScopedInstaller
    {
        public void Install(IScopeBuilder builder, Scene scene)
        {
            // Import application-level services needed by gameplay systems
            builder.Import<ISceneService>();
            builder.Import<ITransitionService>();
            builder.Import<LevelRegistry>();

            // Register gameplay session services
            builder.RegisterSingleton<ILevelManager, LevelManager>();

            // TODO: Register gameplay session adapters and services as they are built
            // e.g., builder.RegisterFromScene<PlayerAdapter>(scene);
            //       builder.RegisterFromScene<CameraAdapter>(scene);
            //       builder.RegisterSingleton<IPlayerService, PlayerService>();
        }
    }
}