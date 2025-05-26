using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class ComponentUtils
    {
        /// <summary>
        ///     Load component of type T if it exists in the GameObject, its children, or its parents.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="result"></param>
        /// <returns> Returns true if load successful. </returns>
        public static bool LoadComponent<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponent<T>();
            if (result != null) return true;
            
            result = component.GetComponentInChildren<T>();
            if (result != null) return true;

            result = component.GetComponentInParent<T>();
            if (result != null) return true;

            return false;
        }


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