using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class TransformUtils
    {
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyTransforms(List<Transform> list)
        {
            if (list == null || list.Count == 0) return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] != null)
                {
                    GameObject.DestroyImmediate(list[i].gameObject);
                }
            }
        }
    }
}