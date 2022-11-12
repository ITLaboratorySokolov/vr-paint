using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

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

        // Deserialize
        ArraySerializer<float> rgbSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] data = rgbSerializer.Deserialize(properties);
        Color[] pixs = ConvertorHelper.FloatsToCol4(data);

        // Set to texture, width and height from TextureSizeProperty
        Texture2D tex = new Texture2D(texSizeProperty.width, texSizeProperty.height);
        tex.SetPixels(pixs);
        tex.anisoLevel = 16;
        tex.Apply();

        // Set texture
        LineRenderer currLine = GetComponent<LineRenderer>(); 
        currLine.material.SetTexture("_MainTex", tex);

        // var mat = GetComponent<MeshRenderer>().material;
        // mat.SetTexture("_MainTex", tex);
    }

    /// <summary>
    /// Serializes the texture property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Get texture
        LineRenderer currLine = GetComponent<LineRenderer>(); 
        Texture tex = currLine.material.GetTexture("_MainTex");
        Texture2D tex2D = null;

        // If no texture is set, send only 0 - it will not be processed anyways since width and height will be set as -1
        float[] data = new float[] { 0 };
        if (tex != null)
        {
            // Convert to Texture2D and get pixels
            tex2D = ConvertorHelper.TextureToTexture2D(tex);
            Color[] pixs = tex2D.GetPixels();
            data = ConvertorHelper.Col4ToFloats(pixs);
        }

        // Serialize
        ArraySerializer<float> rgbSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        byte[] serialized = rgbSerializer.Serialize(data);

        Destroy(tex2D);
        return serialized;
    }

}
