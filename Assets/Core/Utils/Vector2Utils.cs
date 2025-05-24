using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class Vector2Utils
    {
        /// <summary>
        ///     Rotates a 2D vector by a specified angle in degrees and returns the normalized result.
        /// </summary>
        /// <param name="vector"> The vector to rotate. </param>
        /// <param name="angleInDegrees"> The angle to rotate, in degrees. </param>
        /// <returns> The normalized rotated vector. </returns>
        public static Vector2 RotateVector(Vector2 vector, float angleInDegrees)
        {
            // Convert angle from degrees to radians
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // Calculate sine and cosine of the angle
            float sin = Mathf.Sin(angleInRadians);
            float cos = Mathf.Cos(angleInRadians);

            // Apply the 2D rotation matrix
            float rotatedX = (cos * vector.x) - (sin * vector.y);
            float rotatedY = (sin * vector.x) + (cos * vector.y);

            // Return the rotated vector, normalized
            return new Vector2(rotatedX, rotatedY).normalized;
        }


        /// <summary>
        ///     Checks if a given point lies within a circle defined by its center and radius.
        /// </summary>
        /// <param name="center"> The center position of the circle. </param>
        /// <param name="radius"> The radius of the circle. </param>
        /// <param name="point"> The point to test for inclusion within the circle. </param>
        /// <returns>
        ///     True if the point is inside the circle, false otherwise. 
        /// </returns>
        public static bool IsPointInsideCircle(Vector2 center, float radius, Vector2 point)
        {
            return Vector2.Distance(point, center) < radius;
        }
    }
}
