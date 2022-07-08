using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the texture size of the texture set for the drawn line
/// Needs to be applied before TextureProperty
/// </summary>
public class TextureSizeProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "TextureSize";

    /// <summary> Width of texture </summary>
    internal int width;
    /// <summary> Height of texture </summary>
    internal int height;

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming texture size property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the line renderer property then saves those values
    /// - if dictionary does not contain this property, width and height remains -1
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        // Set default values
        width = -1;
        height = -1;

        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize values
        ArraySerializer<int> intSerializer = new ArraySerializer<int>(propertyName, sizeof(int));
        int[] data = intSerializer.Deserialize(properties);

        // If invalid values
        if (data.Length != 2)
            return;

        // Save width and height
        width = data[0];
        height = data[1];
    }

    /// <summary>
    /// Serializes the texture size property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get texture
        LineRenderer currLine = GetComponent<LineRenderer>();
        Texture tex = currLine.material.GetTexture("_MainTex");

        // If texture is not set, values are by default left as -1
        int[] data = new int[] { -1, -1 };
        if (tex != null)
            data = new int[] { tex.width, tex.height };

        // Serialize
        ArraySerializer<int> intSerializer = new ArraySerializer<int>(propertyName, sizeof(int));
        byte[] serialized = intSerializer.Serialize(data);

        return serialized;
    }
}
