using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

public class RigController : MonoBehaviour
{
    [SerializeField]
    GameObject head;
    [SerializeField]
    GameObject lhand;
    [SerializeField]
    GameObject rhand;

    [SerializeField]
    Transform headRig;
    [SerializeField]
    Transform lhandRig;
    [SerializeField]
    Transform rhandRig;

    [SerializeField]
    ObjectController objCont;

    [SerializeField]
    StringVariable clientName;

    internal string handLNM;
    internal string handRNM;
    internal string headNM;

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public async Task SpawnRig()
    {
        handLNM = "HandL_" + clientName.Value;
        handRNM = "HandR_" + clientName.Value;
        headNM = "Head_" + clientName.Value;

        await SpawnRigComponent(lhand, lhandRig, handLNM);
        await SpawnRigComponent(rhand, rhandRig, handRNM);
        await SpawnRigComponent(head, headRig, headNM);

        Debug.Log("Spawned rig");
    }

    private async Task SpawnRigComponent(GameObject prefab, Transform tfParent, string name)
    {
        GameObject o = Instantiate(prefab, tfParent.position, tfParent.rotation, tfParent);
        var uph = o.GetComponent<InputPropertiesHandler>();
        uph.objCont = objCont;
        o.name = name;

        Debug.Log("Spawning " + name);

        // send/update to server
        bool val = await objCont.ContainsObject(name);
        if (val)
        {
            Destroy(o);
            // await objCont.DestroyObject(name);
        }
        else
        {
            await objCont.AddObjectAsync(o);
        }
    }
}
