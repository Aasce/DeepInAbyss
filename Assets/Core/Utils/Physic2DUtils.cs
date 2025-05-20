using System.Collections.Generic;
using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    ///     A utility class providing extended 2D physics operations.
    /// </summary>
    public static class Physic2DUtils
    {
        /// <summary>
        ///     The vertical velocity threshold to determine if the object is considered in the air.
        /// </summary>
        public const float AIR_VELOCITY_Y_LIMIT = 20f;

        /// <summary>
        ///     Performs a filtered 2D raycast from a specified origin in a given direction, with optional filtering rules.
        ///     <br/>
        ///     see <see cref="Physics2D.RaycastAll"/> for more details.
        /// </summary>
        /// <param name="self"> The GameObject performing the raycast (used to skip self-collisions). </param>
        /// <param name="origin"> The starting point of the ray in world coordinates. </param>
        /// <param name="direction"> The direction of the ray. </param>
        /// <param name="distance"> The maximum distance the ray should check for collisions. </param>
        /// <param name="layerMask"> Layer mask that is used to selectively ignore colliders. </param>
        /// <param name="isIgnorePlatform">
        ///     (Optionals) If set to true, platform effectors (like one-way platforms) will be ignored 
        ///     unless hit from above.
        /// </param>
        /// <param name="skipColliders">
        ///     (Optionals) collection of specific colliders to ignore during the raycast.
        /// </param>
        /// <returns>
        ///     The nearest valid <see cref="RaycastHit2D"/> that passes all filters. If no valid collider is hit,
        ///     a default hit is returned with the point set to the maximum ray distance.
        /// </returns>
        public static RaycastHit2D Raycast(this GameObject self, Vector2 origin, Vector2 direction, float distance, LayerMask layerMask,
            bool isIgnorePlatform = false, ICollection<Collider2D> skipColliders = null)
        {
            RaycastHit2D raycastHit = new()
            {
                point = origin + direction * distance
            };

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask);

            if (hits.Length == 0) return raycastHit;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.gameObject == self) continue; // Skip self collide
                if (hit.collider.isTrigger) continue; // Skip trigger
                if (hit.fraction < Mathf.Epsilon) continue; // Skip if too near origin

                // Skip if collider is used by effector and isIgnorePlatform is true 
                if (hit.collider.usedByEffector)
                {
                    if (isIgnorePlatform) continue;
                    if (Vector2.Dot(hit.transform.up, hit.normal) < 0) continue; // only allow collisions from above (Dot > 0).
                }

                // Skip if skipColliders not null and contains this collider
                if (skipColliders != null && skipColliders.Contains(hit.collider)) continue;

                // Find the nearest hit point (Distance is minimum)
                if (Vector3.Distance(hit.point, origin) < Vector3.Distance(raycastHit.point, origin))
                {
                    raycastHit = hit;
                }
            }

            return raycastHit;
        }


    }
}
