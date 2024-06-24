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

        /// <summary>
        /// Returns the object itself if it exists, null otherwise
        /// </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
        /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method
        /// uses Unity's "null check," and if the object has been marked for destruction, it ensures an actual
        /// null reference is returned, aiding in correctly chaining operations and preventing
        /// NullReferenceExceptions
        /// </remarks>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="obj">The object being checked</param>
        /// <returns>The object itself if it exists and is not destroyed, null otherwise</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}