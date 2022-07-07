using System;
using UnityEngine;

/// <summary>
/// Helper class for conversion of arrays
/// </summary>
public static class ConvertorHelper
{
    /// <summary>
    /// Converts array of Color to array of floats
    /// - only takes rgb components
    /// </summary>
    /// <param name="colors"> Array of Color </param>
    /// <returns> Array of floats with rgb components from colors </returns>
    public static float[] Col3ToFloats(Color[] colors)
    {
        float[] floats = new float[colors.Length * 3];

        //Convert each vector to floats
        int index = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            floats[index] = colors[i].r;
            floats[index + 1] = colors[i].g;
            floats[index + 2] = colors[i].b;
            index += 3;
        }

        return floats;
    }


    /// <summary>
    /// Converts array of floats to array of Color
    /// - expects only rbg components
    /// </summary>
    /// <param name="cols"> Array of floats </param>
    /// <returns> Array of colors with values from cols </returns>
    public static Color[] FloatsToCol3(float[] cols)
    {
        if (cols.Length % 3 != 0)
            throw new InvalidOperationException();

        Color[] colors = new Color[cols.Length / 3];

        int index = 0;
        for (int i = 0; i < cols.Length; i += 3)
        {
            colors[index] = new Color(cols[i], cols[i + 1], cols[i + 2]);
            index++;
        }

        return colors;
    }

    /// <summary>
    /// Converts array of floats to array of Vector3
    /// </summary>
    /// <param name="vertices"> Array of floats </param>
    /// <returns> Array of vectors with coordinates from vertices </returns>
    public static Vector3[] FloatsToVec3(float[] vertices)
    {
        if (vertices.Length % 3 != 0)
            throw new InvalidOperationException();

        Vector3[] vectors = new Vector3[vertices.Length / 3];

        int index = 0;
        for (int i = 0; i < vertices.Length; i += 3)
        {
            vectors[index] = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
            index++;
        }

        return vectors;
    }

    /// <summary>
    /// Converts array of Vector3 to array of floats
    /// </summary>
    /// <param name="vecs"> Array of Vector3 </param>
    /// <returns> Float array with coordinates from vectors</returns>
    public static float[] Vec3ToFloats(Vector3[] vecs)
    {
        float[] floats = new float[vecs.Length * 3];

        //Convert each vector to floats
        int index = 0;
        for (int i = 0; i < vecs.Length; i++)
        {
            floats[index] = vecs[i].x;
            floats[index + 1] = vecs[i].y;
            floats[index + 2] = vecs[i].z;
            index += 3;
        }

        return floats;
    }
}