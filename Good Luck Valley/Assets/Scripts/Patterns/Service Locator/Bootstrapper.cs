using GoodLuckValley.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Patterns.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        bool hasBeenBootstrapped;
        ServiceLocator container;

        internal ServiceLocator Container => container.OrNull() ?? (container = GetComponent<ServiceLocator>());

        private void Awake() => BootstrapOnDemand();

        /// <summary>
        /// Initialize the Bootstrapper
        /// </summary>
        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        /// <summary>
        /// Configure the ServiceLocator
        /// </summary>
        protected abstract void Bootstrap();
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator - Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] bool dontDestroyOnLoad = true;

        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(dontDestroyOnLoad);
        }
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator - Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}
