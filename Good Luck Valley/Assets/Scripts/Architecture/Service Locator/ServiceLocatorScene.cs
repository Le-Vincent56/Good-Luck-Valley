using UnityEngine;

namespace GoodLuckValley.Architecture.ServiceLocator
{
    [AddComponentMenu("Service Locator/Scene Service Locator")]
    public class ServiceLocatorScene : Bootstrapper
    {
        /// <summary>
        /// Bootstrap by configuring the Service Locator for scene use
        /// </summary>
        protected override void Bootstrap() => Container.ConfigureForScene();
    }
}