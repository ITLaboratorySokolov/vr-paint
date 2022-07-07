using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

public class LineRendererProperty : OptionalProperty
{
    string propertyName = "LineRenderer";

    RawMeshSerializer serializer = new RawMeshSerializer();

    public override string GetPropertyName()
    {
        return propertyName;
    }

    public override void Process(Dictionary<string, byte[]> properties)
    {
        Debug.Log("Process");
        // Deseralize properties from obj
        var indices = serializer.IndicesSerializer.Deserialize(properties);
        var vertices = serializer.VerticesSerializer.Deserialize(properties);
        Vector3[] vectors = ConvertorHelper.FloatsToVec3(vertices);

        // Update line properties - vertices and indices
        LineRenderer l = GetComponent<LineRenderer>();
        MeshCollider meshCollider = l.GetComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        mesh.SetVertices(vectors);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        meshCollider.sharedMesh = mesh;
        // recieve vertices and indices and set them to line renderer
    }


    public override byte[] Serialize()
    {
        Debug.Log("Serialize");

        // do nothing
        ArraySerializer<float> rgbSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] color = new float[] { 0 };
        byte[] serialized = rgbSerializer.Serialize(color);
        return serialized;
    }

}
