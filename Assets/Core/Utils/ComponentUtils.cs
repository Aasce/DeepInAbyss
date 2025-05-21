using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class ComponentUtils
    {
        /// <summary>
        ///     Get component of type T if it exists, otherwise add it to the GameObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return component;
        }

        /// <summary>
        ///     Get component of type T if it exists, otherwise add it to the GameObject.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.GetOrAddComponent<T>();
        }

        /// <summary>
        ///     Set sorting Layer and Order for all renderers in the GameObject and its children.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="layerName"></param>
        /// <param name="order"></param>
        public static void SetSortingLayerAndOrder(this Component self, string layerName, int order)
        {
            if (self == null) return;
            Renderer[] renderers = self.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                // Set sorting layer and order for each renderer
                renderer.sortingLayerName = layerName ?? "Default";
                renderer.sortingOrder = order;
            }
        }
    }
}