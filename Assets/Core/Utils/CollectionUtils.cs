using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        ///     Iterates over a sequence in chunks, yielding after processing a specified number of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="collection">The enumerable collection to iterate over.</param>
        /// <param name="chunkSize">Number of items to process per frame.</param>
        /// <param name="action">Action to perform on each item.</param>
        public static IEnumerator IterateInChunks<T>(IEnumerable<T> collection, int chunkSize, Action<T> action, float? delay = null)
        {
            if (collection == null || action == null || chunkSize <= 0)
                yield break;

            IList<T> list = collection is IList<T> collectionList ? collectionList : new List<T>(collection);
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                action.Invoke(list[i]);

                if ((i + 1) % chunkSize == 0)
                    yield return delay.HasValue && delay.Value > 0 ? new WaitForSeconds(delay.Value) : null;
            }

            if (count % chunkSize != 0)
                yield return delay.HasValue && delay.Value > 0 ? new WaitForSeconds(delay.Value) : null;
        }

        /// <summary>
        ///     Iterates over a sequence across a fixed number of frames, distributing the work evenly.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection to process.</param>
        /// <param name="totalFrames">The total number of frames to spread processing over.</param>
        /// <param name="action">The action to apply to each element.</param>
        public static IEnumerator IterateOverFrames<T>(IEnumerable<T> collection, int totalFrames, Action<T> action, float? delay = null)
        {
            if (collection == null || action == null || totalFrames <= 0)
                yield break;

            IList<T> list = collection is IList<T> collectionList ? collectionList : new List<T>(collection);
            int totalItems = list.Count;

            if (totalItems == 0)
                yield break;

            int baseChunkSize = totalItems / totalFrames;
            int remainder = totalItems % totalFrames;

            int index = 0;

            for (int frame = 0; frame < totalFrames && index < totalItems; frame++)
            {
                int chunkSize = baseChunkSize + (frame < remainder ? 1 : 0);
                int end = index + chunkSize;

                for (; index < end && index < totalItems; index++)
                    action.Invoke(list[index]);

                yield return delay.HasValue && delay.Value > 0 ? new WaitForSeconds(delay.Value) : null;
            }
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

        public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
        {
            if (dict == null || dict.Count == 0) return default;
            return dict.FirstOrDefault(pair => EqualityComparer<TValue>.Default.Equals(pair.Value, value)).Key;
        }
    }
}