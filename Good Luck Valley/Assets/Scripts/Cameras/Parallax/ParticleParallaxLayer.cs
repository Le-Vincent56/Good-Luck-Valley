using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParticleParallaxLayer : ParallaxLayer
    {
        private ParallaxSystem system;

        private void Start()
        {
            // Register this to be tracked by the Parallax System
            system = ServiceLocator.ForSceneOf(this).Get<ParallaxSystem>();
            system.RegisterLayer(this);
        }

        private void OnDestroy()
        {
            // Exit case - if the Parallax System is null
            if (system == null) return;

            // Deregister this from being tracked by the Parallax System
            system.DeregisterLayer(this);
        }
    }
}
