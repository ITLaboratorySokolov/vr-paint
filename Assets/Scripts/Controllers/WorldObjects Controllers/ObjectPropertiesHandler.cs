using UnityEngine;

/// <summary>
/// Script used to set properties of incoming game object from the server
/// - properties are based on the name of the game object
/// </summary>
public class ObjectPropertiesHandler : MonoBehaviour
{
    [SerializeField]
    public ObjectController objCont;
    [SerializeField]
    public ServerConectionController serverConnection;

    [SerializeField]
    Material lineMaterial;
    [SerializeField]
    Material boxMaterial;
    [SerializeField]
    Material generalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color c = mr.material.GetColor("_Color"); ;
        Texture t = mr.material.GetTexture("_MainTex");

        // object is a line
        if (name.StartsWith("Line"))
        {
            mr.material = lineMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
            int layerNum = LayerMask.NameToLayer("DrawnLine");
            gameObject.layer = layerNum;
            gameObject.tag = "line";

            // GetComponent<Rigidbody>().useGravity = false;
        }
        // object is a box
        else if (name.StartsWith("CardboardBox"))
        {
            mr.material = boxMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
        }
        // object is a general object
        else
        {
            mr.material = generalMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
        }

    }

}
