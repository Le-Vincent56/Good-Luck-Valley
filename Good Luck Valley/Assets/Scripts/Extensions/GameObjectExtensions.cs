using UnityEngine;

namespace GoodLuckValley.Extensions.GameObjects
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns the object itself if it exists, null otherwise
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
        /// </summary>
        /// <remarks>
        /// This method is useful when you don't know if a GameObject has a specific type of component,
        /// but you want to work with that component regardless. Instead of checking and adding the component manually,
        /// you can use this method to do both operations in one line.
        /// </remarks>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to get the component from or add the component to.</param>
        /// <returns>The existing component of the given type, or a new one if no such component exists.</returns>    
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }
    }
}