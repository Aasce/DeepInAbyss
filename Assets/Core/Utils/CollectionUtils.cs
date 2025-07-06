using System;
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
        ///     Creates a list of a specified size and fills it using a creation function or default values.
        /// </summary>
        /// <typeparam name="T"> The type of elements to create. </typeparam>
        /// <param name="size"> The number of elements to generate in the list. </param>
        /// <param name="createFunc">
        ///     Optional. A function that generates elements based on their index.
        ///     If null, default values for type T will be used.
        /// </param>
        /// <returns>
        ///     A list of T elements with the specified size, filled with either default values or generated values.
        /// </returns>
        public static List<T> CreateWithSize<T>(int size, Func<int, T> createFunc = null)
        {
            if (size < 0) return new List<T>();

            List<T> list = new(size);

            for (int i = 0; i < size; i++)
            {
                T newElement;
                if (createFunc == null) newElement = default;
                else newElement = createFunc(i);
                list.Add(newElement);
            }

            return list;
        }


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
            int index = UnityEngine.Random.Range(0, collection.Count);
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
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}