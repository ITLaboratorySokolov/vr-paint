using System;
using UnityEngine;

/// <summary>
/// Helper class for conversion of arrays
/// </summary>
public static class ConvertorHelper
{
    /// <summary>
    /// Elongate array by add number of members, does not initialize the new members
    /// </summary>
    /// <typeparam name="T"> Type </typeparam>
    /// <param name="array"> Array </param>
    /// <param name="add"> Number of members to add </param>
    /// <returns> New, longer array </returns>
    public static T[] ElongateArray<T>(T[] array, int add)
    {
        T[] newArr = new T[array.Length + add];
        for (int i = 0; i < array.Length; i++)
            newArr[i] = array[i];

        return newArr;
    }

    /// <summary>
    /// Converts array of Color to array of floats
    /// - takes rgba components
    /// </summary>
    /// <param name="colors"> Array of Color </param>
    /// <returns> Array of floats with rgb components from colors </returns>
    public static float[] Col4ToFloats(Color[] colors)
    {
        float[] floats = new float[colors.Length * 4];

        //Convert each vector to floats
        int index = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            floats[index] = colors[i].r;
            floats[index + 1] = colors[i].g;
            floats[index + 2] = colors[i].b;
            floats[index + 3] = colors[i].a;
            index += 4;
        }

        return floats;
    }

    /// <summary>
    /// Converts array of floats to array of Vector2
    /// </summary>
    /// <param name="vertices"> Array of floats </param>
    /// <returns> Array of vectors2 </returns>
    internal static Vector2[] FloatToVec2(float[] vals)
    {
        if (vals.Length % 2 != 0)
            throw new InvalidOperationException();

        Vector2[] res = new Vector2[vals.Length / 2];

        int index = 0;
        for (int i = 0; i < vals.Length; i += 2)
        {
            res[index] = new Vector2(vals[i], vals[i + 1]);
            index++;
        }

        return res;
    }


    /// <summary>
    /// Converts array of floats to array of Color
    /// - expects rbga components
    /// </summary>
    /// <param name="cols"> Array of floats </param>
    /// <returns> Array of colors with values from cols </returns>
    public static Color[] FloatsToCol4(float[] cols)
    {
        if (cols.Length % 4 != 0)
            throw new InvalidOperationException();

        Color[] colors = new Color[cols.Length / 4];

        int index = 0;
        for (int i = 0; i < cols.Length; i += 4)
        {
            colors[index] = new Color(cols[i], cols[i + 1], cols[i + 2], cols[i + 3]);
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

    /// <summary>
    /// Converts array of Vector2 to array of floats
    /// </summary>
    /// <param name="vecs"> Array of Vector2 </param>
    /// <returns> Float array with coordinates from vectors</returns>
    public static float[] Vec2ToFloats(Vector2[] vecs)
    {
        float[] floats = new float[vecs.Length * 2];

        //Convert each vector to floats
        int index = 0;
        for (int i = 0; i < vecs.Length; i++)
        {
            floats[index] = vecs[i].x;
            floats[index + 1] = vecs[i].y;
            index += 2;
        }

        return floats;
    }

    public static Texture2D texture2D;
    static RenderTexture currentRT;
    static RenderTexture renderTexture;

    /// <summary>
    /// Converts Texture to Texture2D
    /// </summary>
    /// <param name="texture"> Source texture </param>
    /// <returns> Resulting texture </returns>
    public static Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        // texture2D.Reinitialize(texture.width, texture.height); 

        currentRT = RenderTexture.active;
        renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.active = currentRT;

        UnityEngine.Object.Destroy(currentRT);
        return texture2D;
    }
}
