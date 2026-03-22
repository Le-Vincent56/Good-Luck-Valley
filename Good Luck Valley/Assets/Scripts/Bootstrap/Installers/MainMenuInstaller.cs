using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.SceneManagement.Interfaces;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Bootstrap.Installers
{
    /// <summary>
    /// Scoped installer for the Main Menu scene. Creates a container scoped
    /// under the Application Container. Imports ISceneService so menu UI can trigger scene transitions
    /// (e.g., "Start Game")
    /// </summary>
    public class MainMenuInstaller : IScopedInstaller
    {
        public void Install(IScopeBuilder builder, Scene scene)
        {
            builder.Import<ISceneService>();
            
            // TODO: Register menu-specific services and adapters as they are built
            // e.g., builder.RegisterSingleton<IMenuController, MenuController>();
            //       builder.RegisterFromScene<MenuAdapter>(scene);
        }
    }
}