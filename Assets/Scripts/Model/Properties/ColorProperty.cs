using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the color of the drawn line
/// </summary>
public class ColorProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "Color";

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming color property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the color property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize color
        ArraySerializer<float> rgbSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] color = rgbSerializer.Deserialize(properties);
        Color colorRGB = new Color(color[0], color[1], color[2]);

        Debug.Log(colorRGB);

        // Set color to mesh renderer
        var mat = GetComponent<MeshRenderer>().material;
        mat.SetColor("_Color", colorRGB);
    }

    /// <summary>
    /// Serializes the color property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get color
        var mat = GetComponent<MeshRenderer>().material;
        Color currC = mat.GetColor("_Color");
        float[] color = ConvertorHelper.Col4ToFloats(new Color[]{ currC });
        
        // Serialize color
        ArraySerializer<float> rgbSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        byte[] serialized = rgbSerializer.Serialize(color);

        return serialized;
    }

}
