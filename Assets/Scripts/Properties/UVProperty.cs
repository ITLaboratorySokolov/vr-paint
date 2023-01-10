using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the UVs of the drawn line
/// </summary>
public class UVProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "UVindices";

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming UV property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the color property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize color
        ArraySerializer<float> uvSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] vals = uvSerializer.Deserialize(properties);
        Vector2[] uvs = ConvertorHelper.FloatToVec2(vals);

        // Set color to mesh renderer
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.uv = uvs;
    }

    /// <summary>
    /// Serializes the UV property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get color
        var mesh = GetComponent<MeshFilter>().mesh;
        float[] vals = ConvertorHelper.Vec2ToFloats(mesh.uv);

        // Serialize color
        ArraySerializer<float> uvSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        byte[] serialized = uvSerializer.Serialize(vals);

        return serialized;
    }


}
