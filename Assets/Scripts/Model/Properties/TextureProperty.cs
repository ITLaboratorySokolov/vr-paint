using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the texture set for the drawn line
/// Needs to be applied after TextureSizeProperty
/// </summary>
public class TextureProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "Texture";

    /// <summary> Texture size property used for this game object </summary>
    [SerializeField]
    TextureSizeProperty texSizeProperty;

    /// <summary> Texture format property for this game object </summary>
    [SerializeField]
    TextureFormatProperty texFormatProperty;

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
    /// Processes incoming texture property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the texture property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName) || texSizeProperty.width <= 0 || texSizeProperty.height <= 0)
            return;

        // Set to texture, width and height from TextureSizeProperty
        Texture2D tex = new Texture2D(texSizeProperty.width, texSizeProperty.height, texFormatProperty.texFormat, false);
        tex.SetPixelData(properties[propertyName], 0);
        tex.Apply();

        Debug.Log("TexSet");

        // Set texture
        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture(textureName, tex);
    }

    /// <summary>
    /// Serializes the texture property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get texture
        Texture tex = GetComponent<MeshRenderer>().material.GetTexture(textureName);
        Texture2D tex2D = null;

        // If no texture is set, send only 0 - it will not be processed anyways since width and height will be set as -1
        byte[] data = new byte[] { 0 };
        if (tex != null)
        {
            // Convert to Texture2D and get pixel data
            tex2D = ConvertorHelper.TextureToTexture2D(tex);
            data = tex2D.GetPixelData<byte>(0).ToArray();
        }

        Destroy(tex2D);
        return data;
    }

}
