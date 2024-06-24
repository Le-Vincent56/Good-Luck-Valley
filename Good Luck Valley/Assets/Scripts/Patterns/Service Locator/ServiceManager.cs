using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Patterns.ServiceLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        public IEnumerable<object> RegisteredServices => services.Values;

        /// <summary>
        /// Register a service to the Service Manager
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <param name="service">The service to register</param>
        /// <returns>The ServiceManager where the service was registered to</returns>
        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            // Try to add the service
            if(!services.TryAdd(type, service))
            {
                // Log if the service is already registered
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            return this;
        }

        /// <summary>
        /// Register a service to the Service Manager
        /// </summary>
        /// <param name="type">The type of the service</param>
        /// <param name="service">The service to register</param>
        /// <returns>The ServiceManager where the service was registered to</returns>
        public ServiceManager Register(Type type, object service)
        {
            // Verify that the type corresponds with the service
            if(!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            // Try to add the service
            if(!services.TryAdd(type, service))
            {
                // Log if the service is already registered
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            }

            return this;
        }

        /// <summary>
        /// Get a service of type T from the ServiceManager
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The service as type T</returns>
        public T Get<T>() where T : class
        {
            // Get the type fo the service
            Type type = typeof(T);

            // Try to get the service
            if(services.TryGetValue(type, out object obj))
            {
                // Return the service as type T
                return obj as T;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            // Get the type of the service
            Type type = typeof(T);
            
            // Try to get the service
            if (services.TryGetValue(type, out object obj))
            {
                // Return the service as type T
                service = obj as T;
                return true;
            }

            // Otherwise, set the service to null and return false
            service = null;
            return false;
        }
    }
}
