using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class LayerUtils
    {
        /// <summary>
        ///     Checks if a specific layer index is included in a given LayerMask.
        /// </summary>
        public static bool IsInLayerMask(int layer, LayerMask mask)
        {
            return ((1 << layer) & mask) != 0;
        }

        /// <summary>
        ///     Checks if the GameObject's layer is included in the given LayerMask.
        /// </summary>
        public static bool IsInLayerMask(GameObject obj, LayerMask mask)
        {
            return IsInLayerMask(obj.layer, mask);
        }

        /// <summary>
        ///     Checks if the layer of a Collider2D's GameObject is included in the given LayerMask.
        /// </summary>
        public static bool IsInLayerMask(Collider2D collider, LayerMask mask)
        {
            return IsInLayerMask(collider.gameObject.layer, mask);
        }

        public static int LayerToInt(LayerMask layer)
        {
            int value = layer.value;
            if (value != 0 && (value & (value - 1)) == 0)
            {
                return (int)Mathf.Log(value, 2);
            }
            return LayerMask.NameToLayer("Default");
        }
    }
}