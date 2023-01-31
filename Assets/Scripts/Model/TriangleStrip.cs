using System.Collections.Generic;
using UnityEngine;

public class TriangleStrip
{

    public List<Vector3> positions;
    public Mesh mesh;

    public TriangleStrip(Vector3 startPos)
    {
        positions = new List<Vector3>();
        Vector3 p2 = new Vector3(startPos.x + 0.001f, startPos.y, startPos.z);
        Vector3 p3 = new Vector3(startPos.x - 0.001f, startPos.y, startPos.z);

        positions.Add(startPos);

        mesh = new Mesh();
        mesh.vertices = new Vector3[] { startPos, p2, p3 };
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.uv = new Vector2[] { new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f) };
    }
    
}