using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    /// Provides extension methods for collections, such as retrieving a random element.
    /// </summary>
    public static class CollectionUtils
    {

        /// <summary>
        ///     Returns a random element from an <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T"> The type of elements in the collection. </typeparam>
        /// <param name="collection"> The collection to select a random element from. </param>
        /// <returns>
        ///     A randomly selected element from the collection. Returns default(T) if the collection is null or empty.
        /// </returns>
        public static T GetRandomElement<T>(this ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0) return default;

            // If the collection is a list, delegate to the optimized IList version
            if (collection is IList<T> list) return list.GetRandomElement();

            // Otherwise, pick a random index manually
            int index = Random.Range(0, collection.Count);
            int currentIndex = 0;

            // Iterate through the collection to find the element at the random index
            foreach (T item in collection)
            {
                if (currentIndex == index)
                    return item;
                currentIndex++;
            }

            // Fallback in case something goes wrong (shouldn't normally happen)
            return default;
        }

        /// <summary>
        ///     Returns a random element from an <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T"> The type of elements in the list. </typeparam>
        /// <param name="list"> The list to select a random element from. </param>
        /// <returns>
        ///     A randomly selected element from the list. Returns default(T) if the list is null or empty.
        /// </returns>
        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return default;

            // Use UnityEngine.Random to pick a random index
            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}