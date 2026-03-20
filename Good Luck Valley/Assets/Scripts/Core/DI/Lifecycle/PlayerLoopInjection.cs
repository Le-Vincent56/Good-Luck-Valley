using System;
using System.Collections.Generic;
using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Lifecycle
{
    /// <summary>
    /// Inserts a custom phase into Unity's PlayerLoop that runs
    /// </summary>
    internal static class PlayerLoopInjection
    {
        private static readonly Queue<PendingInstallation> Pending = new Queue<PendingInstallation>();
        private static bool _initialized;
        
        /// <summary>
        /// Marker struct used to identify the DI injection phase in the PlayerLoop.
        /// </summary>
        private struct DIInjectionPhase { }

        /// <summary>
        /// Inserts the DI injection phase into Unity's EarlyUpdate loop.
        /// Safe to call multiple times - only initializes once.
        /// </summary>
        public static void Initialize()
        {
            if(_initialized) return;
            _initialized = true;
            
            PlayerLoopSystem currentLoop = PlayerLoop.GetCurrentPlayerLoop();
            InsertIntoEarlyUpdate(ref currentLoop);
            PlayerLoop.SetPlayerLoop(currentLoop);
        }

        /// <summary>
        /// Queues a scene installation to be processed in the next EarlyUpdate frame.
        /// </summary>
        /// <param name="installation">The pending installation to process.</param>
        public static void Enqueue(PendingInstallation installation) => Pending.Enqueue(installation);

        /// <summary>
        /// Processes all pending installations from the queue.
        /// Each installation is either initialized as a root container or a scoped container,
        /// depending on the presence of a parent scope.
        /// Exceptions during the installation process are logged.
        /// </summary>
        private static void ProcessPending()
        {
            while (Pending.Count > 0)
            {
                PendingInstallation pending = Pending.Dequeue();

                try
                {
                    IContainer container;

                    if (pending.ParentScope != null)
                    {
                        // Scoped container for an additive scene
                        IScopeBuilder scopeBuilder = pending.ParentScope.CreateScope(pending.Scene.name);
                        pending.ScopedInstaller.Install(scopeBuilder, pending.Scene);
                        container = scopeBuilder.Build();
                    }
                    else
                    {
                        // Root container for a base scene
                        ContainerBuilder builder = new ContainerBuilder(pending.Scene.name);
                        pending.Installer.Install(builder, pending.Scene);
                        container = builder.Build();
                    }

                    ContainerRegistry.RegisterSceneContainer(pending.Scene, container);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        /// <summary>
        /// Inserts a custom DI injection phase into Unity's EarlyUpdate loop.
        /// Modifies the PlayerLoop structure to include the injection phase at the start of EarlyUpdate.
        /// This ensures injection runs before ScriptRunDelayedStartupFrame (which triggers Start())
        /// </summary>
        /// <param name="loop">The current PlayerLoop system to modify.</param>
        private static void InsertIntoEarlyUpdate(ref PlayerLoopSystem loop)
        {
            PlayerLoopSystem injectionSystem = new PlayerLoopSystem
            {
                type = typeof(DIInjectionPhase),
                updateDelegate = ProcessPending
            };

            PlayerLoopSystem[] topLevelSystems = loop.subSystemList;

            for (int i = 0; i < topLevelSystems.Length; i++)
            {
                if (topLevelSystems[i].type != typeof(UnityEngine.PlayerLoop.EarlyUpdate))
                    continue;
                
                PlayerLoopSystem earlyUpdate = topLevelSystems[i];
                PlayerLoopSystem[] existingSubs = earlyUpdate.subSystemList;

                if (existingSubs == null)
                {
                    earlyUpdate.subSystemList = new PlayerLoopSystem[] { injectionSystem };
                }
                else
                {
                    PlayerLoopSystem[] newSubs = new PlayerLoopSystem[existingSubs.Length + 1];
                    newSubs[0] = injectionSystem;
                    Array.Copy(existingSubs, 0, newSubs, 1, existingSubs.Length);
                    earlyUpdate.subSystemList = newSubs;
                }

                topLevelSystems[i] = earlyUpdate;
                return;
            }
        }
    }
}