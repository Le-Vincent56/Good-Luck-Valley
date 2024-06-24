using GoodLuckValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Patterns.ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator global;
        private static Dictionary<Scene, ServiceLocator> sceneContainers;
        private static List<GameObject> tmpSceneGameObjects;

        private readonly ServiceManager services = new ServiceManager();

        private const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        private const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";


        public static ServiceLocator Global
        {
            get
            {
                if (global != null) return global;

                // Find the first Service Locator Global Bootstrapper
                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found) {
                    // Initialize it and return it
                    found.BootstrapOnDemand();
                    return global;
                }

                // Otherwise, create a container and add the Service locator Global Bootstrapper and initialize it
                GameObject container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return global;
            }
        }

        /// <summary>
        /// Configure the Serivce Locator as global
        /// </summary>
        /// <param name="dontDestroyOnLoad">Whether or not to destroy on load or not</param>
        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            }
            else if (global != null)
            {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            }
            else
            {
                global = this;
                if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Configure the Service Locator for a scene
        /// </summary>
        internal void ConfigureForScene()
        {
            // Get the current scene
            Scene scene = gameObject.scene;
            
            // Check if another ServiceLocator is already configured for the current scene
            if(sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("SerivceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }

            // Add the ServiceLocator and configure it to the current scene
            sceneContainers.Add(scene, this);
        }

        /// <summary>
        /// Try to get a ServiceLocator with priority on the parent GameObject of a MonoBehaviour
        /// </summary>
        /// <param name="mb">The Monobehaviour to check the parents of</param>
        /// <returns></returns>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
        }

        /// <summary>
        /// Try to get a ServiceLocator from a Scene of a given MonoBehaviour
        /// </summary>
        /// <param name="mb">The MonoBehaviour to check the scene for</param>
        /// <returns></returns>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            // Get the current scene of the GameObject
            Scene scene = mb.gameObject.scene;

            if(sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
            {
                return container;
            }

            // Clear all temporary scene GameObjects
            tmpSceneGameObjects.Clear();

            // Get all of the root game objects and store them
            scene.GetRootGameObjects(tmpSceneGameObjects);

            // Iterate through each game object that has a Service Locator Scene Bootstrapper
            foreach (GameObject gameObject in tmpSceneGameObjects.Where(gameObject => gameObject.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                // Try to get a Service Locator Scene Bootstrapper and check if the container of that Bootstrapper does not equal
                // the given MonoBehaviour
                if (gameObject.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    // If so, initialize the Bootstrapper and return the ServiceLocator
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            // Use the global if a Service Locator Scene Bootstrapper could not be found
            return Global;
        }

        /// <summary>
        /// Register a service with the Service Locator
        /// </summary>
        /// <typeparam name="T">The type of the service to register</typeparam>
        /// <param name="service">The service to register</param>
        /// <returns>The Service Locator that registered the service</returns>
        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }

        /// <summary>
        /// Register a service with the Service Locator
        /// </summary>
        /// <param name="type">The type of the service to register</param>
        /// <param name="service">The service to register</param>
        /// <returns>The Service Locator that registered the service</returns>
        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type, service);
            return this;
        }

        /// <summary>
        /// Try to get a service from the Service Locator
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <param name="service">The service to get</param>
        /// <returns>The Service Locator that located the service</returns>
        /// <exception cref="ArgumentException"></exception>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            // Try to get the service from the Service Manager
            if (TryGetService(out service)) return this;

            // Try to get the service from the hierarchy
            if(TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>
        /// Try to get a service from the Service Locator
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The found service of type T</returns>
        /// <exception cref="ArgumentException"></exception>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            // Try to get the service from the Service Manager
            if (TryGetService(type, out T service)) return service;

            // Try to get the service from the Hierarchy
            if (TryGetNextInHierarchy(out ServiceLocator container))
                return container.Get<T>();

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// Try to get a service from the Service Manager
        /// </summary>
        /// <typeparam name="T">The type of the service to get</typeparam>
        /// <param name="service">The retrieved service</param>
        /// <returns>True if the service was successfully retrieved, false if otherwise</returns>
        bool TryGetService<T>(out T service) where T : class
        {
            return services.TryGet(out service);
        }

        bool TryGetService<T>(Type type, out T service) where T : class
        {
            return services.TryGet(out service);
        }

        /// <summary>
        /// Try to get a service throughout the hierarchy
        /// </summary>
        /// <param name="container">The Service Locator holding the service</param>
        /// <returns>True if the service has been found, false if otherwise</returns>
        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            // Check if we are at the global level
            if(this == global)
            {
                // Set the container to null and return false
                // because a service has not yet been found
                // and there's no further check in the hierarchy
                container = null;
                return false;
            }

            // Go through the hierarchy (from parent, to scene, to global) and check for a Service Locator
            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        private void OnDestroy()
        {
            if(this == global)
            {
                global = null;
            } else if(sceneContainers.ContainsValue(this))
            {
                sceneContainers.Remove(gameObject.scene);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tmpSceneGameObjects = new List<GameObject>();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal()
        {
            GameObject gameObject = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene()
        {
            GameObject gameObject = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif
    }
}
