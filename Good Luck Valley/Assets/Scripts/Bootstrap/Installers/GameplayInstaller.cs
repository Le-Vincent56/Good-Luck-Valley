using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using GoodLuckValley.Core.Utilities;
using GoodLuckValley.Gameplay.Player.Adapters;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Handlers;
using GoodLuckValley.Gameplay.Player.Interfaces;
using GoodLuckValley.Gameplay.Player.Motor;
using GoodLuckValley.Gameplay.Player.Services;
using GoodLuckValley.World.LevelManagement.Data;
using GoodLuckValley.World.LevelManagement.Interfaces;
using GoodLuckValley.World.LevelManagement.Services;
using UnityEngine.SceneManagement;
using SceneUtility = GoodLuckValley.Core.Utilities.SceneUtility;

namespace GoodLuckValley.Bootstrap.Installers
{
    /// <summary>
    /// Scoped installer for the Persistent Gameplay Scene. Creates a container
    /// scoped under the Application Container that lives for the entire gameplay
    /// session. Level containers are scoped children of this container.
    /// </summary>
    public class GameplayInstaller : IScopedInstaller
    {
        public void Install(IScopeBuilder builder, Scene scene)
        {
            // Import application-level services
            builder.Import<ISceneService>();
            builder.Import<ITransitionService>();
            builder.Import<LevelRegistry>();
            builder.Import<IPlayerInput>();

            // Register gameplay session services
            builder.RegisterSingleton<ILevelManager, LevelManager>();

            // Player system — data from scene
            PlayerAdapter playerAdapter = SceneUtility.FindComponentInScene<PlayerAdapter>(scene);
            builder.RegisterInstance<PlayerStats>(playerAdapter.Stats);
            builder.RegisterInstance<CharacterSize>(playerAdapter.CharacterSize);
            builder.RegisterFromScene<PlayerAdapter>(scene);

            // Player system — simulation
            builder.RegisterSingleton<IPlayerMotor, PlayerMotor>();
            builder.RegisterSingleton<IJumpHandler, PlayerJumpHandler>();
            builder.RegisterSingleton<IWallHandler, PlayerWallHandler>();
            builder.RegisterSingleton<IBounceHandler, PlayerBounceHandler>();
            builder.RegisterSingleton<ICrawlHandler, PlayerCrawlHandler>();
            builder.RegisterSingleton<IPlayerService, PlayerService>();
        }
    }
}