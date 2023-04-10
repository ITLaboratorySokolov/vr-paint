using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a triangle strip
/// </summary>
public class TriangleStrip
{
    /// <summary> List of point positions </summary>
    public List<Vector3> positions;
    /// <summary> Created triangle strip </summary>
    public Mesh mesh;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    public TriangleStrip(Vector3 startPos)
    {
        positions = new List<Vector3>();

        positions.Add(startPos);

        mesh = new Mesh();
        mesh.vertices = new Vector3[] { startPos };
        mesh.triangles = new int[] {  };
        mesh.uv = new Vector2[] { new Vector2(0, 0.5f) };
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="startPos"> Starting position </param>
    /// <param name="orientation"> Orientation </param>
    /// <param name="w"> Width </param>
    public TriangleStrip(Vector3 startPos, Vector3 orientation, float w)
    {
        positions = new List<Vector3>();

        Vector3 p1 = startPos + w/2 * orientation;
        Vector3 p2 = startPos - w/2 * orientation;
        positions.Add(p1);
        positions.Add(p2);

        mesh = new Mesh();
        mesh.vertices = new Vector3[] { p1, p2 };
        mesh.triangles = new int[] { };
        mesh.uv = new Vector2[] { new Vector2(0, 1), new Vector2(0, 0) };
    }

}