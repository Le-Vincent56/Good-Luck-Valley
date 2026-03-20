using System;
using System.Collections.Generic;
using System.Reflection;
using GoodLuckValley.Core.DI.Attributes;
using GoodLuckValley.Core.DI.Exceptions;
using UnityEngine;

namespace GoodLuckValley.Core.DI.Injection
{
    /// <summary>
    /// Static cache for type injection metadata. Reflection occurs once per type at container
    /// build time, then all subsequent lookups are dictionary hits. Zero runtime reflection
    /// after initial caching.
    /// </summary>
    internal static class InjectionCache
    {
        private static readonly Dictionary<Type, TypeInjectionInfo> InjectionInfoCache = new Dictionary<Type, TypeInjectionInfo>();
        private static readonly Dictionary<Type, ConstructorInfo> ConstructorCache = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// Returns cached [Inject] field/property metadata for the given type.
        /// If not yet cached, reflects on the type once and stores the result.
        /// </summary>
        /// <param name="type">The type for which injection metadata is to be retrieved or created.</param>
        /// <returns>The <see cref="TypeInjectionInfo"/> containing injection metadata for the specified type.</returns>
        public static TypeInjectionInfo GetOrCreate(Type type)
        {
            if (!InjectionInfoCache.TryGetValue(type, out TypeInjectionInfo info))
            {
                info = BuildInjectionInfo(type);
                InjectionInfoCache.Add(type, info);
            }

            return info;
        }

        /// <summary>
        /// Returns the cached constructor for greedy constructor injection (most parameter wins).
        /// Used for pure C# simulation-layer classes only.
        /// </summary>
        /// <param name="type">The type for which the constructor information is to be retrieved.</param>
        /// <returns>The <see cref="ConstructorInfo"/> representing the resolved constructor of the specified type.</returns>
        public static ConstructorInfo GetConstructor(Type type)
        {
            if (!ConstructorCache.TryGetValue(type, out ConstructorInfo constructor))
            {
                constructor = ResolveConstructor(type);
                ConstructorCache[type] = constructor;
            }

            return constructor;
        }

        /// <summary>
        /// Clears all cached data. Used for test teardown and domain reload safety.
        /// </summary>
        internal static void Clear()
        {
            InjectionInfoCache.Clear();
            ConstructorCache.Clear();
        }

        private static TypeInjectionInfo BuildInjectionInfo(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            FieldInfo[] allFields = type.GetFields(flags);
            List<FieldInfo> injectableFields = new List<FieldInfo>();
            
            // Collect all fields marked with [Inject]
            foreach (FieldInfo field in allFields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() == null) continue;
                
                injectableFields.Add(field);
            }
            
            PropertyInfo[] allProperties = type.GetProperties(flags);
            List<PropertyInfo> injectableProperties = new List<PropertyInfo>();
            
            // Collect all properties marked with [Inject]
            foreach (PropertyInfo property in allProperties)
            {
                // Skip if the property is not marked with [Inject] or is not writable
                if (property.GetCustomAttribute<InjectAttribute>() == null || !property.CanWrite) continue;
                
                injectableProperties.Add(property);
            }
            
            return new TypeInjectionInfo(type, injectableFields.ToArray(), injectableProperties.ToArray());
        }

        /// <summary>
        /// Resolves the most suitable constructor for the specified type by analyzing its constructor definitions.
        /// Throws an exception if no public constructors are found or if the type is a MonoBehaviour.
        /// </summary>
        /// <param name="type">The type for which to resolve the appropriate constructor.</param>
        /// <returns>The <see cref="ConstructorInfo"/> representing the most parameter-rich public constructor of the specified type.</returns>
        /// <exception cref="InjectionException">
        /// Thrown if the type is a MonoBehaviour, as constructor injection is not supported for MonoBehaviour types.
        /// Thrown if no public constructors are found on the specified type.
        /// </exception>
        private static ConstructorInfo ResolveConstructor(Type type)
        {
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                throw new InjectionException(
                    $"Cannot use constructor injection for MonoBehaviour type '{type.Name}'. " +
                    $"MonoBehaviours must use [Inject] field injection via RegisterFromScene<{type.Name}>()."
                );
            }
            
            ConstructorInfo[] constructors = type.GetConstructors();
            
            if (constructors.Length == 0)
            {
                throw new InjectionException(
                    $"No public constructors found on type '{type.Name}'. " +
                    $"Ensure the type has at least one public constructor for dependency injection."
                );
            }
            
            // Select the constructor with the most parameters
            ConstructorInfo bestGreedy = constructors[0];
            for(int i = 1; i < constructors.Length; i++)
            {
                if (constructors[i].GetParameters().Length <= bestGreedy.GetParameters().Length) continue;
                
                bestGreedy = constructors[i];
            }

            return bestGreedy;
        }
    }
}