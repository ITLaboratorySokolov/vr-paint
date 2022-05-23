using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Utility
{
    /// <summary>
    /// Converts points to floats and vice versa.
    /// </summary>
    public static class PointConverter
    {
        /// <summary>
        /// Converts array of vectors to array of floats.
        /// </summary>
        /// <param name="points">Array of vectors.</param>
        /// <returns>Array of floats.</returns>
        public static float[] Point3DToFloat(Vector3[] points)
        {
            int index = 0;
            var coordinates = new float[points.Length * 3];

            foreach (var point in points)
            {
                coordinates[index++] = point.x;
                coordinates[index++] = point.y;
                coordinates[index++] = point.z;
            }

            return coordinates;
        }

        /// <summary>
        /// Converts array of floats to array of vectors.
        /// </summary>
        /// <param name="coordinates">Array of floats.</param>
        /// <returns>Array of vectors.</returns>
        public static Vector3[] FloatToPoint3D(float[] coordinates)
        {
            int index = 0;
            var points = new Vector3[coordinates.Length / 3];

            for (int i = 0; i < coordinates.Length; i += 3)
            {
                points[index++] = new Vector3(coordinates[i], coordinates[i + 1], coordinates[i + 2]);
            }

            return points;
        }
    }
}
