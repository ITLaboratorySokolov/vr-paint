using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleStripGenerator
{

    Vector3[] newVertices;
    int[] newTriangles;

    /// <summary>
    /// Add new point newPoint to the line represented by a triangle strip
    /// </summary>
    /// <param name="newPoint"> New point to add </param>
    /// <param name="width"> Width of the line at this point </param>
    /// <param name="orientation"> Orientation of the line plane, normalized vector </param>
    /// <param name="strip"> Strip representing the line </param>
    /// <returns></returns>
    public TriangleStrip AddPointToLine(Vector3 newPoint, float width, Vector3 orientation, TriangleStrip strip)
    {
        newVertices = ConvertorHelper.ElongateArray<Vector3>(strip.mesh.vertices, 2);

        // add two points, each +-width/2 from newPos in orientation
        Vector3 p1 = newPoint + width / 2 * orientation;
        Vector3 p2 = newPoint - width / 2 * orientation;

        newVertices[strip.mesh.vertices.Length] = p1;
        newVertices[strip.mesh.vertices.Length + 1] = p2;

        // creating the first triangle
        if (strip.mesh.vertices.Length == 1)
        {
            // create one new triangle
            newTriangles = ConvertorHelper.ElongateArray<int>(strip.mesh.triangles, 3);

            newTriangles[strip.mesh.triangles.Length] = strip.mesh.vertices.Length - 1;
            newTriangles[strip.mesh.triangles.Length + 1] = strip.mesh.vertices.Length;
            newTriangles[strip.mesh.triangles.Length + 2] = strip.mesh.vertices.Length + 1;
        }
        else
        {
            // create two new triangles
            newTriangles = ConvertorHelper.ElongateArray<int>(strip.mesh.triangles, 2 * 3);

            newTriangles[strip.mesh.triangles.Length] = strip.mesh.vertices.Length - 2;
            newTriangles[strip.mesh.triangles.Length + 1] = strip.mesh.vertices.Length + 1;
            newTriangles[strip.mesh.triangles.Length + 2] = strip.mesh.vertices.Length - 1;

            newTriangles[strip.mesh.triangles.Length + 3] = strip.mesh.vertices.Length - 2;
            newTriangles[strip.mesh.triangles.Length + 4] = strip.mesh.vertices.Length;
            newTriangles[strip.mesh.triangles.Length + 5] = strip.mesh.vertices.Length + 1;
        }

        strip.mesh.vertices = newVertices;
        strip.mesh.triangles = newTriangles;

        // TODO UVs

        return strip;
    }

}
