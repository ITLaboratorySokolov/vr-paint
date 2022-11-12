using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the texture size of the texture set for the drawn line
/// Needs to be applied before TextureProperty
/// </summary>
public class LineWidthFuncProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "LineWidthFunc";

    /// <summary> Animation curve </summary>
    AnimationCurve curve;

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming width curve size property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the line width func property then saves those values
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        curve = null;
        LineRenderer currLine = GetComponent<LineRenderer>();

        if (!properties.ContainsKey(propertyName))
        {
            // Debug.Log("No WidthCurve - adding");
            // AnimationCurve curve = new AnimationCurve();
            // curve.AddKey(0, 1);
            // curve.AddKey(1, 1);
            // currLine.widthCurve = curve;
            return;
        }

        Debug.Log("Processing WidthCurve");

        // Deserialize values
        ArraySerializer<float> floatSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] data = floatSerializer.Deserialize(properties);

        // If invalid values
        if (data.Length % 2 != 0)
            return;

        // Add keyframes
        curve = new AnimationCurve();
        for (int i = 0; i < data.Length; i += 2)
        {
            Debug.Log(data[i] + " " + data[i + 1]);
            int r = curve.AddKey(data[i], data[i + 1]);
        }
        currLine.widthCurve = curve;
    }

    /// <summary>
    /// Serializes the width curveproperty, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get width curve
        LineRenderer currLine = GetComponent<LineRenderer>();
        curve = currLine.widthCurve;
        Keyframe[] keyframes = curve.keys;

        float[] data = new float[2 * keyframes.Length];
        int index = 0;

        // Get data
        for (int i = 0; i < keyframes.Length; i++)
        {
            data[index] = keyframes[i].time;
            data[index + 1] = keyframes[i].value;

            index += 2;
        }
        
        // Serialize
        ArraySerializer<float> floatsSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        byte[] serialized = floatsSerializer.Serialize(data);

        return serialized;
    }
}
