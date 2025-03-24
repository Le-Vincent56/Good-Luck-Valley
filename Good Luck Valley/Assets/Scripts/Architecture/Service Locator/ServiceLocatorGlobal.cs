using UnityEngine;

namespace GoodLuckValley.Architecture.ServiceLocator
{
    [AddComponentMenu("Service Locator/Global Service Locator")]
    public class ServiceLocatorGlobal : Bootstrapper
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        /// <summary>
        /// Bootstrap by configuring the Service Locator for global use
        /// </summary>
        protected override void Bootstrap() => Container.ConfigureAsGlobal(dontDestroyOnLoad);
    }
}