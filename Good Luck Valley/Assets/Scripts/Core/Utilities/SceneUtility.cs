using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Core.Utilities
{
    /// <summary>
    /// General-purpose scene search utilities. Uses scene-scoped search via
    /// <c>scene.GetRootGameObjects()</c> and <c>GetComponentInChildren</c>,
    /// avoiding global <c>FindObjectsByType</c> calls that scan all loaded scenes.
    /// </summary>
    public static class SceneUtility
    {
        /// <summary>
        /// Searches for a component of type <typeparamref name="T"/> within the root GameObjects of the specified scene.
        /// The search includes all child GameObjects recursively and can find inactive components.
        /// </summary>
        /// <typeparam name="T">The type of the component to find.</typeparam>
        /// <param name="scene">The scene to search for the component.</param>
        /// <returns>The first component of type <typeparamref name="T"/> found in the scene, or null if no such component exists.</returns>
        public static T FindComponentInScene<T>(Scene scene) where T : Component
        {
            GameObject[] roots = scene.GetRootGameObjects();

            for (int i = 0; i < roots.Length; i++)
            {
                T component = roots[i].GetComponentInChildren<T>(true);
                
                if (!component) continue;
                
                return component;
            }
            
            return null;
        }

        /// <summary>
        /// Finds all components of type T in the given scene's hierarchy.
        /// Searches all root GameObjects and their children, including inactive objects.
        /// </summary>
        /// <param name="scene">The scene to search within.</param>
        /// <typeparam name="T">The component type to search for.</typeparam>
        /// <returns>A list of all matching components. May be empty.</returns>
        public static List<T> FindAllComponentsInScene<T>(Scene scene) where T : Component
        {
            List<T> results = new List<T>();
            GameObject[] roots = scene.GetRootGameObjects();

            for (int i = 0; i < roots.Length; i++)
            {
                T[] components = roots[i].GetComponentsInChildren<T>(true);
                results.AddRange(components);
            }
            
            return results;
        }
    }
}