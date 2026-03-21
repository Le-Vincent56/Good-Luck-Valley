using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;
using UnityEngine;

namespace GoodLuckValley.Bootstrap
{
    /// <summary>
    /// Composition root for Good Luck Valley. Builds the application-scoped container
    /// at startup via [RuntimeInitializeOnLoadMethod]. No MonoBehaviour or scene object required.
    /// </summary>
    public static class Bootstrapper
    {
        /// <summary>
        /// Builds the application-scoped container before any scene loads.
        /// Application services persist for the entire game session.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            ContainerBuilder appBuilder = new ContainerBuilder("Application");
            
            // TODO: Application-scoped services are registered here as they are built

            IContainer appContainer = appBuilder.Build();
            ContainerRegistry.SetApplicationContainer(appContainer);
        }

        /// <summary>
        /// Installs the initial scene's container after Unity loads the first scene.
        /// Awake() has already fired; Start() fires next frame - injection slots in between.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnInitialSceneLoaded()
        {
            // TODO: Install the initial scene's container once scene installers are created
        }
    }
}