using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UpdatePropertiesHandler : MonoBehaviour
{
    float timeUntilUpdate;

    [SerializeField]
    public ObjectController objCont;
    [SerializeField]
    public ServerConectionController serverConnection;

    Vector3 newPos;
    Vector3 lastPos;

    Quaternion newRot;
    Quaternion lastRot;

    [SerializeField]
    TextureProperty texProp;

    [SerializeField]
    Material lineMaterial;
    [SerializeField]
    Material boxMaterial;
    [SerializeField]
    Material generalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        timeUntilUpdate = 0;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Color c = mr.material.GetColor("_Color"); ;
        Texture t = mr.material.GetTexture("_MainTex");

        //texProp.textureName = "_MainTex";

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
        else if (name.StartsWith("CardboardBox"))
        {
            mr.material = boxMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
        }
        else
        {
            mr.material = generalMaterial;
            mr.material.SetColor("_Color", c);
            if (t != null)
                mr.material.SetTexture("_MainTex", t);
        }

    }

    public void StartPosition()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }   

}
