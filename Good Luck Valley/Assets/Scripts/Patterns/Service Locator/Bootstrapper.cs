using UnityEngine;
using GoodLuckValley.Extensions.GameObject;

namespace GoodLuckValley.Patterns.ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator container;
        private bool hasBeenBootstrapped;

        internal ServiceLocator Container => container.OrNull() ?? (container = GetComponent<ServiceLocator>());

        private void Awake()
        {
            BootstrapOnDemand();
        }

        /// <summary>
        /// Force the bootstrap
        /// </summary>
        public void BootstrapOnDemand()
        {
            // Exit case - if already been bootstrapped
            if (hasBeenBootstrapped) return;

            // Bootstrap
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        /// <summary>
        /// Bootstrap the Service Locator
        /// </summary>
        protected abstract void Bootstrap();
    }
}