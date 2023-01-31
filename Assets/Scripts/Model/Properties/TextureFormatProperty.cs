using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the texture format of the texture
/// Needs to be applied before TextureProperty
/// </summary>
public class TextureFormatProperty : OptionalProperty
{
    /// <summary> Processed texture format </summary>
    internal TextureFormat texFormat;
    
    /// <summary> Property name </summary>
    string propertyName = "TextureFormat";

    /// <summary> Name of texture </summary>
    [SerializeField]
    string textureName = "_MainTex";

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming texture format property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the texture format property then saves the property value into texFormat
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize values
        StringSerializer strSerializer = new StringSerializer(propertyName);
        string data = strSerializer.Deserialize(properties);

        if (data.Length < 3)
            return;

        texFormat = ConvertFormatFromString(data);
        Debug.Log(texFormat);
    }

    /// <summary>
    /// Serializes the texture format property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        Texture tex = GetComponent<MeshRenderer>().material.GetTexture(textureName);
        Texture2D tex2D = null;

        // If no texture is set, send only 0 - it will not be processed anyways since width and height will be set as -1
        byte[] data = new byte[] { 0 };
        if (tex != null)
        {
            // Convert to Texture2D and get pixels
            tex2D = ConvertorHelper.TextureToTexture2D(tex);
            string f = ConvertFormatToString(tex2D.format);

            StringSerializer strSerializer = new StringSerializer(propertyName);
            data = strSerializer.Serialize(f);
        }

        return data;
    }


    /// <summary>
    /// Converts a pixel format from a string to TextureFormat.
    /// </summary>
    /// <param name="format">The pixel format.</param>
    /// <returns>The texture format.</returns>
    /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
    private TextureFormat ConvertFormatFromString(string format)
    {
        switch (format)
        {
            case "RGBA": return TextureFormat.RGBA32;
            case "ARGB": return TextureFormat.ARGB32;
            case "RGB": return TextureFormat.RGB24;
            default: throw new ArgumentException($"Unsupported texture format: {format}");
        }
    }

    /// <summary>
    /// Converts a pixel format from TextureFormat to a string.
    /// </summary>
    /// <param name="format">The texture format.</param>
    /// <returns>The pixel fromat in a string.</returns>
    /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
    private string ConvertFormatToString(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.RGBA32: return "RGBA";
            case TextureFormat.ARGB32: return "ARGB";
            case TextureFormat.RGB24: return "RGB";
            default: throw new ArgumentException($"Unsupported texture format: {format}");
        }
    }
}
