using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Models.Utility
{
    /// <summary>
    /// Converts points to floats and vice versa.
    /// </summary>
    public static class VectorConverter
    {
        /// <summary>
        /// Converts array of vectors to array of floats.
        /// </summary>
        /// <param name="vectors">Array of vectors.</param>
        /// <returns>Array of floats.</returns>
        public static float[] Vector3ToFloat(Vector3[] vectors)
        {
            return VectorsToArray(vectors, 3, (i, v) => v[i]);
        }

        /// <summary>
        /// Converts array of floats to array of vectors.
        /// </summary>
        /// <param name="coordinates">Array of floats.</param>
        /// <returns>Array of vectors.</returns>
        public static Vector3[] FloatToVector3(float[] coordinates)
        {
            return ArrayToVectors(coordinates, 3, (i, coords) => new Vector3(coords[i], coords[i + 1], coords[i + 2]));
        }

        public static float[] Vector2ToFloat(Vector2[] vectors)
        {
            return VectorsToArray(vectors, 2, (i, v) => v[i]);
        }

        public static Vector2[] FloatToVector2(float[] coordinates)
        {
            return ArrayToVectors(coordinates, 2, (i, coords) => new Vector2(coords[i], coords[i + 1]));
        }

        private static R[] VectorsToArray<R, T>(IReadOnlyCollection<T> vectors, int vectorSize, Func<int, T, R> convertFunc)
        {
            var index = 0;
            var array = new R[vectors.Count * vectorSize];

            foreach (var vector in vectors)
            {
                for (var i = 0; i < vectorSize; i++)
                {
                    array[index++] = convertFunc(i, vector);
                }
            }

            return array;
        }

        private static R[] ArrayToVectors<R, T>(IReadOnlyList<T> array, int vectorSize, Func<int, IReadOnlyList<T>, R> convertFunc)
        {
            var index = 0;
            var vectors = new R[array.Count / vectorSize];

            for (var i = 0; i < array.Count; i += vectorSize)
            {
                vectors[index++] = convertFunc(i, array);
            }

            return vectors;
        }
    }
}