using System;
using System.Collections.Generic;
using System.Linq;
using GoodLuckValley.Extensions.GameObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Architecture.ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator global;
        private static Dictionary<Scene, ServiceLocator> sceneContainers;
        private static List<GameObject> tempSceneGameObjects;
        private readonly ServiceManager services = new();

        const string k_globalServiceLocatorName = "Service Locator [Global]";
        const string k_sceneServiceLocatorName = "Service Locator [Scene]";

        private void OnDestroy()
        {
            // Check if this Service Locator was the global one
            if (this == global)
                // If so, nullify the global Service Locator
                global = null;
            // Check if this Service Locator is attached to a scene
            else if (sceneContainers.ContainsValue(this))
                // If so, remove the Service Locator from its scene
                sceneContainers.Remove(gameObject.scene);
        }

        /// <summary>
        /// Configure the Service Locator for global use
        /// </summary>
        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            // Check if the Service Locator is already configured as global
            if (global == this)
            {
                // Debug
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal(): " +
                    "Already configurted as global", this);
            }
            // Check if the Service Locator exists, but does not equal this Service Locator
            else if (global != null)
            {
                // Debug
                Debug.LogError("ServiceLocator.ConfigureAsGlobal(): " +
                    "Another ServiceLocator is already configured as global", this);
            }
            else
            {
                // Configure this service locator for global use
                global = this;

                // Verify to not destroy on load
                if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Configure the Service Locator for scene use
        /// </summary>
        internal void ConfigureForScene()
        {
            // Get the current scene
            Scene scene = gameObject.scene;

            // Exit case - if the scene containers already contains this scene
            if (sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator.ConfigureForScene(): " +
                    "Another ServiceLocator is already configured for this scene", this);

                return;
            }

            // Link this Service Locator to the current scene
            sceneContainers.Add(scene, this);
        }

        /// <summary>
        /// Get the Global Service Locator instance; creates a new one if none exists
        /// </summary>
        public static ServiceLocator Global
        {
            get
            {
                // Exit case - if there is already a global Service Locator
                if (global != null) return global;

                // Try to find a Global Service Locator
                if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    // If found, bootstrap it
                    found.BootstrapOnDemand();
                    return global;
                }

                // Create a new game object
                GameObject container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));

                // Add a global Service Locator component and bootstrap it
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return global;
            }
        }

        /// <summary>
        /// Returns the Service Locator configured for the scene of a given MonoBehaviour; falls back to the Global Service Locator
        /// </summary>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            Scene scene = mb.gameObject.scene;

            // Check if a scene Service Locator exists for this scene and if it is not the same as the given MonoBehaviour
            if (sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
                // If so, return the found Service Locator
                return container;

            // Clear the temporary scene GameObjects
            tempSceneGameObjects.Clear();

            // Store all of the root GameOjects
            scene.GetRootGameObjects(tempSceneGameObjects);

            // Loop through each root GameObject where a scene Service Locator exists
            foreach (GameObject gameObject in
                tempSceneGameObjects.Where(gameObject => gameObject.GetComponent<ServiceLocatorScene>() != null))
            {
                // Check if the GameObject has a valid scene bootstrapper commponent and the attached Service Locator
                // is not the same as the given MonoBehaviour
                if (gameObject.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
                {
                    // Bootstrap the Service Locator and return it
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            // If nothing was found, return the global Service Locator
            return Global;
        }

        /// <summary>
        /// Gets the closest Service Locator instance to the provided MonoBehaviour in the hierarchy, the
        /// Service Locator for its scene, or the global Service Locator
        /// </summary>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
        }

        /// <summary>
        /// Registers a service to the Service Locator using the service's type
        /// </summary>
        public ServiceLocator Register<T>(T service) where T : class
        {
            services.Register(service);
            return this;
        }

        /// <summary>
        /// Registers a service to the Service Locator using a specific type
        /// </summary>
        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type, service);
            return this;
        }

        /// <summary>
        /// Gets a service of a specific type coupled with the Service Locator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            // Check if the service could be successfully retrieved
            if (TryGetService(out service)) return this;

            // Check if a Service Locator could be found through the hierarchy
            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                // If so, get and set the service
                container.Get(out service);
                return this;
            }

            // If no service of the required type is found, throw an error
            throw new ArgumentException($"ServiceLocator.Get(): Service of type '{typeof(T).FullName}' not registered");
        }

        /// <summary>
        /// Get an isolated instance of a service of a specific type
        /// </summary>
        public T Get<T>() where T : class
        {
            // Get the type of the service
            Type type = typeof(T);

            // State a null service
            T service = null;

            // Check if the service could be successfully retrieved
            if (TryGetService(type, out service))
                // If so, return it
                return service;

            // Check if the Service Locator could be succesfully retrieved through the hierarchy
            if (TryGetNextInHierarchy(out ServiceLocator container))
                // If so, return it
                return container.Get<T>();

            throw new ArgumentException($"ServiceLocator.Get(): Could not resolve type '{typeof(T).FullName}'");
        }

        /// <summary>
        /// Try to get a service from the Service Locator
        /// </summary>
        private bool TryGetService<T>(out T service) where T : class => services.TryGet(out service);

        /// <summary>
        /// Try to get s service from the Service Locator using a specific type
        /// </summary>
        private bool TryGetService<T>(Type type, out T service) where T : class => services.TryGet(out service);

        /// <summary>
        /// Try to get the next Service Locator found in the hierarchy
        /// </summary>
        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            // Check if this Service Locator is the global one
            if (this == global)
            {
                // If so, set the container to null and return
                // unsuccessful
                container = null;
                return false;
            }

            // Check if the parent object has a Service Locator for this scene and set it
            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);

            // Return whether or not a Service Locator was found
            return container != null;
        }

        /// <summary>
        /// Reset static structures
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tempSceneGameObjects = new List<GameObject>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Create a global Service Locator object
        /// </summary>
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            GameObject go = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobal));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        /// <summary>
        /// Create a scene Service Locator object
        /// </summary>
        private static void AddScene()
        {
            GameObject go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorScene));
        }
#endif
    }
}