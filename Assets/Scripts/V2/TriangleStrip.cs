using System.Collections.Generic;
using UnityEngine;

public class TriangleStrip
{

    public List<Vector3> positions;
    public Mesh mesh;

    public TriangleStrip(Vector3 startPos)
    {
        positions = new List<Vector3>();
        positions.Add(startPos);

        mesh = new Mesh();
        mesh.vertices = new Vector3[] { startPos };
        mesh.triangles = new int[] { };
        mesh.uv = new Vector2[] { new Vector2(0, 0.4f) };
    }
    
}