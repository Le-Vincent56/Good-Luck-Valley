using System.Collections.Generic;
using GoodLuckValley.Core.DI.Exceptions;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.DI.Injection
{
    /// <summary>
    /// Handles scene-scoped MonoBehaviour discovery and [Inject] field injection.
    /// All searches are scoped to a specific scene via <c>scene.GetRootGameObjects()</c>,
    /// avoiding global <c>FindObjectsByType</c> calls that scan all loaded scenes.
    /// </summary>
    internal static class MonoBehaviourInjector
    {
        /// <summary>
        /// Finds the first MonoBehaviour of type T in the given scene's hierarchy.
        /// Searches root GameObjects and their children, including inactive objects.
        /// Stops at the first match.
        /// </summary>
        /// <param name="scene">The scene to search within.</param>
        /// <typeparam name="T">The MonoBehaviour type to search for.</typeparam>
        /// <returns>The first matching component found.</returns>
        /// <exception cref="InjectionException">Thrown if no component of type T exists in the scene.</exception>
        public static T FindInScene<T>(Scene scene) where T : MonoBehaviour
        {
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                T component = root.GetComponentInChildren<T>(true);

                if (component != null) return component;
            }

            throw new InjectionException(
                $"Could not find {typeof(T).Name} in scene '{scene.name}'. " +
                $"Ensure the MonoBehaviour exists in the scene and is registered " +
                $"in the installer via RegisterFromScene<{typeof(T).Name}>()."
            );
        }

        /// <summary>
        /// Finds all MonoBehaviours of type T in the given scene's hierarchy.
        /// Searches all root GameObjects and their children, including inactive objects.
        /// </summary>
        /// <param name="scene">The scene to search within.</param>
        /// <typeparam name="T">The MonoBehaviour type to search for.</typeparam>
        /// <returns>A list of all matching components found. May be empty.</returns>
        public static List<T> FindAllInScene<T>(Scene scene) where T : MonoBehaviour
        {
            List<T> results = new List<T>();
            GameObject[] roots = scene.GetRootGameObjects();

            foreach (GameObject root in roots)
            {
                T[] components = root.GetComponentsInChildren<T>(true);
                results.AddRange(components);
            }

            return results;
        }

        /// <summary>
        /// Injects all [Inject]-marked fields and properties on the target instance using
        /// the given container to resolve dependencies. Used by <see cref="Factory.PrefabFactory{T}"/>
        /// for runtime-spawned objects.
        /// </summary>
        /// <param name="target">The object instance to inject into.</param>
        /// <param name="container">The container to resolve dependencies from.</param>
        public static void InjectFields(object target, IContainer container)
        {
            TypeInjectionInfo info = InjectionCache.GetOrCreate(target.GetType());

            if (!info.HasInjectables) return;
            
            info.InjectInto(target, container.Resolve);
        }
    }
}