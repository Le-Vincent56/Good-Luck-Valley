using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ExternalParallaxLayer : ParallaxLayer
    {
        private ParallaxSystem system;

        private void OnEnable()
        {
            // Get the parallax system if not retrieved already
            if (system == null) system = ServiceLocator.ForSceneOf(this).Get<ParallaxSystem>();

            // Initialize the layer
            Initialize();

            // Register this to be tracked by the Parallax System
            system.RegisterLayer(this);
        }

        private void OnDisable()
        {
            // Get the parallax system if not retrieved already
            if (system == null) system = ServiceLocator.ForSceneOf(this).Get<ParallaxSystem>();

            // Deregister this from being tracked by the Parallax System
            system.DeregisterLayer(this);
        }

        private void OnDestroy()
        {
            // Get the parallax system if not retrieved already
            if (system == null) system = ServiceLocator.ForSceneOf(this).Get<ParallaxSystem>();

            // Deregister this from being tracked by the Parallax System
            system.DeregisterLayer(this);
        }

        /// <summary>
        /// Set the multiplier for the external parallax layer
        /// </summary>
        public void SetMultiplier(float multiplier) => this.multiplier = multiplier;
    }
}
