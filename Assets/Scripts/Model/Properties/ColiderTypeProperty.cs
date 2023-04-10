using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the type of colider of the object
/// Needs to be applied before ColiderProperty
/// </summary>
public class ColiderTypeProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "ColiderType";

    [Header("Colliders")]
    [SerializeField()]
    BoxCollider boxCol;
    [SerializeField()]
    SphereCollider sphCol;
    [SerializeField()]
    MeshCollider meshCol;

    /// <summary> Type of property </summary>
    internal string type;

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming colider type property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the property then saves those values
    /// - if dictionary does not contain this property, colider type remains "box"
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        // Deserialize values
        StringSerializer stringSerializer = new StringSerializer(propertyName);
        string inType = stringSerializer.Deserialize(properties);
        
        if (inType == "sphere")
        {
            boxCol.enabled = false;
            sphCol.enabled = true;
            meshCol.enabled = false;
        }
        else if (inType == "mesh")
        {
            boxCol.enabled = false;
            sphCol.enabled = false;
            meshCol.enabled = true;

            meshCol.sharedMesh = GetComponent<MeshFilter>().mesh;
        }

        type = inType;
    }

    /// <summary>
    /// Serializes the property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        // Set default values
        type = "no";

        if (boxCol.enabled)
            type = "box";
        else if (sphCol.enabled)
            type = "sphere";
        else if (meshCol.enabled)
            type = "mesh";

        // Serialize values
        StringSerializer stringSerializer = new StringSerializer(propertyName);
        byte[] serialized  = stringSerializer.Serialize(type);

        return serialized;
    }
}
