using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Architecture.ServiceLocator
{
    public class ServiceManager
    {
        private readonly Dictionary<Type, object> services = new();
        public IEnumerable<object> RegisteredServices => services.Values;

        /// <summary>
        /// Try to get a service
        /// </summary>
        public bool TryGet<T>(out T service) where T : class
        {
            // Get the type of the service
            Type type = typeof(T);

            // Check if the service type exists in the dictionary
            if (services.TryGetValue(type, out object obj))
            {
                // If so, set the service and return successful
                service = obj as T;
                return true;
            }

            // If no type was found, set the service to null
            // and return unsuccessful
            service = null;
            return false;
        }

        /// <summary>
        /// Get a service
        /// </summary>
        public T Get<T>() where T : class
        {
            // Get the type of the service
            Type type = typeof(T);

            // Check if the service type exists in the dictionary
            if (services.TryGetValue(type, out object obj))
            {
                // If so, return the service
                return obj as T;
            }

            // If no type was found, throw an exception
            throw new ArgumentException($"ServiceManager.Get(): Service of type '{type.FullName}' not registered");
        }

        /// <summary>
        /// Register a service to the Service Manager
        /// </summary>
        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            // Check if adding the service was unsuccessful
            if (!services.TryAdd(type, service))
                // Debug an error
                Debug.LogError($"ServiceManager.Register(): Service of type '{type.FullName}' already registered");

            // If it was successful, return this object
            return this;
        }

        /// <summary>
        /// Register a service to the Service Manager
        /// </summary>
        public ServiceManager Register(Type type, object service)
        {
            // Check if the type is the same type as the service
            if (!type.IsInstanceOfType(service))
                // If not, throw an exception
                throw new ArgumentException($"ServiceManager.Register(): " +
                    $"Type of service does not match type of service interface", nameof(service));

            // Check if adding the service was unsuccessful
            if (!services.TryAdd(type, service))
                // Debug an error
                Debug.LogError($"ServiceManager.Register(): Service of type '{type.FullName}' already registered");

            // If it was successful, return this object
            return this;
        }
    }
}