using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the positions set in the line renderer
/// </summary>
public class LineRendererProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "LineRenderer";

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming line renderer property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the line renderer property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deseralize properties from obj
        ArraySerializer<float> posSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] data = posSerializer.Deserialize(properties);
        Vector3[] vecs = ConvertorHelper.FloatsToVec3(data);

        // Update line properties - positions
        LineRenderer l = GetComponent<LineRenderer>();
        l.positionCount = vecs.Length;
        l.SetPositions(vecs);
    }


    /// <summary>
    /// Serializes the line renderer property (in detail, the line positions), called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get positions of line
        LineRenderer l = GetComponent<LineRenderer>();
        Vector3[] pos = new Vector3[l.positionCount];
        l.GetPositions(pos);

        // Serialize
        ArraySerializer<float> posSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        byte[] serialized = posSerializer.Serialize(ConvertorHelper.Vec3ToFloats(pos));

        return serialized;
    }

}
