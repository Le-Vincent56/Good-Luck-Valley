using UnityEngine;

namespace GoodLuckValley.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Get a Component of type T on a GameObject, or add it if it doesn't have one
        /// </summary>
        /// <typeparam name="T">The type of Component</typeparam>
        /// <param name="gameObject">The GameObject to check</param>
        /// <returns>The Component of type T</returns>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            // Get the component
            T component = gameObject.GetComponent<T>();

            // If there's not a component, add the component
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }
    }
}