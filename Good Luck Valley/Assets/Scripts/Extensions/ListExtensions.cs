using System.Collections.Generic;

namespace GoodLuckValley.Extensions.List
{
    public static class ListExtensions
    {
        /// <summary>
        /// Refresh the list with a new set of items
        /// </summary>
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items)
        {
            // Clear the list
            list.Clear();

            // Add all of the given items
            list.AddRange(items);
        }
    }
}