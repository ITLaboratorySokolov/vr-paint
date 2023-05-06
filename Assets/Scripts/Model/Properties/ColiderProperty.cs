using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Properties;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Serializers;

/// <summary>
/// Custom property processing the size and position of colider of the object
/// </summary>
public class ColiderProperty : OptionalProperty
{
    /// <summary> Property name </summary>
    string propertyName = "ColiderSize";

    [Header("Colliders")]
    [SerializeField()]
    BoxCollider boxCol;
    [SerializeField()]
    SphereCollider sphCol;
    [SerializeField()]
    MeshCollider meshCol;

    /// <summary> Float array serializer </summary>
    ArraySerializer<float> floatSerializer;

    /// <summary>
    /// Getter for property name
    /// </summary>
    /// <returns> Property name </returns>
    public override string GetPropertyName()
    {
        return propertyName;
    }

    /// <summary>
    /// Processes incoming Colider center positon and size property, called when object is recieved from the server
    /// - recieves properties dictionary, and if it contains the color property then sets the property onto the game object
    /// </summary>
    /// <param name="properties"> Properties dictionary </param>
    public override void Process(Dictionary<string, byte[]> properties)
    {
        if (!properties.ContainsKey(propertyName))
            return;

        floatSerializer = new ArraySerializer<float>(propertyName, sizeof(float));
        float[] vals = floatSerializer.Deserialize(properties);

        if (meshCol.enabled)
        {
            // meshCol.convex = true;
            boxCol.center = new Vector3();
            boxCol.size = new Vector3();
            sphCol.center = new Vector3();
            sphCol.radius = 0;

            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
        }

        // if sphere colider
        else if (sphCol.enabled)
        {
            // Deserialize center and radius
            Vector3 c = new Vector3(vals[0], vals[1], vals[2]);

            sphCol.center = c;
            sphCol.radius = vals[3];

            boxCol.center = new Vector3();
            boxCol.size = new Vector3();
        }

        // if box colider
        else if (boxCol.enabled)
        {
            // Deserialize size and center
            Vector3[] vecs = ConvertorHelper.FloatsToVec3(vals);

            // Set color to mesh renderer
            boxCol.center = vecs[0];
            boxCol.size = vecs[1];

            sphCol.center = new Vector3();
            sphCol.radius = 0;
        }
    }

    /// <summary>
    /// Serializes the center position and size property, called when object is sent to the server
    /// - gets the value of the property and returns serialized byte array value
    /// </summary>
    /// <returns> Byte array with value of the property </returns>
    public override byte[] Serialize()
    {
        if (meshCol.enabled)
            return new byte[0];

        byte[] serialized = new byte[0];
        floatSerializer = new ArraySerializer<float>(propertyName, sizeof(float));

        if (boxCol.enabled)
        {
            float[] vals = ConvertorHelper.Vec3ToFloats(new Vector3[] { boxCol.center, boxCol.size });

            // Serialize color
            serialized = floatSerializer.Serialize(vals);
        }

        if (sphCol.enabled)
        {
            float[] vals = new float[] { sphCol.center.x, sphCol.center.y, sphCol.center.z, sphCol.radius };

            // Serialize color
            serialized = floatSerializer.Serialize(vals);
        }

        return serialized;
    }
}
