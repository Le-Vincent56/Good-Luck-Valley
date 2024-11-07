using UnityEngine;

namespace GoodLuckValley.Extensions.GameObject
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns the object itself if it exists, null otherwise
        /// </summary>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
    }
}