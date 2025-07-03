using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class CameraUtils
    {
        public static Bounds GetBounds(this Camera camera)
        {
            if (camera == null) return new Bounds();
            Vector3 center = camera.transform.position;
            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            float far = camera.farClipPlane;

            return new Bounds(center, new Vector3(width, height, far));
        }
    }
}