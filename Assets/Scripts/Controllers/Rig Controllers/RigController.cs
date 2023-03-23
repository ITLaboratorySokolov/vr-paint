using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;
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

    internal string handLNM;
    internal string handRNM;
    internal string headNM;

    GameObject lHandObj;
    GameObject rHandObj;
    GameObject headObj;

    // Start is called before the first frame update
    void Start()
    {
        // XRGeneralSettings.Instance.Manager.StartSubsystems();
        StartCoroutine(StartXR());

        // spawn rig components
        SpawnRig();
        SwapColor(false);
    }

    IEnumerator StartXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwapColor(bool connected)
    {
        if (lHandObj == null)
            lHandObj = GameObject.Find(handLNM);
        if (rHandObj == null)
            rHandObj = GameObject.Find(handRNM);

        Material lMat = lHandObj.GetComponent<MeshRenderer>().material;
        Material rMat = rHandObj.GetComponent<MeshRenderer>().material;

        if (connected)
        {
            lMat.color = Color.green;
            rMat.color = Color.green;
        }
        else
        {
            lMat.color = Color.red;
            rMat.color = Color.red;
        }

        Debug.Log("Swapped color");
    }

    public void SpawnRig()
    {
        // spawn to default position?
        handLNM = "HandL_" + objCont.clientName.Value;
        handRNM = "HandR_" + objCont.clientName.Value;
        headNM = "Head_" + objCont.clientName.Value;

        // left hand
        lHandObj = SpawnRigComponent(lhand, lhandRig, handLNM);
        rHandObj = SpawnRigComponent(rhand, rhandRig, handRNM);
        headObj = SpawnRigComponent(head, headRig, headNM);
    }

    private GameObject SpawnRigComponent(GameObject prefab, Transform tfParent, string name)
    {
        GameObject o = Instantiate(prefab, tfParent.position, tfParent.rotation, tfParent);
        var uph = o.GetComponent<InputPropertiesHandler>();
        uph.objCont = objCont;
        o.name = name;

        Debug.Log("Spawning " + name);

        Mesh mesh = o.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(mesh.triangles, 0);
        mesh.subMeshCount = 1;

        MeshRenderer meshRend = o.GetComponent<MeshRenderer>();
        Material[] mats = new Material[] { meshRend.material };
        meshRend.materials = mats;

        return o;
    }

    public async Task AddRigToServer()
    {
        handLNM = "HandL_" + objCont.clientName.Value;
        handRNM = "HandR_" + objCont.clientName.Value;
        headNM = "Head_" + objCont.clientName.Value;

        if (lHandObj == null)
            lHandObj = SpawnRigComponent(lhand, lhandRig, handLNM);
        if (rHandObj == null)
            rHandObj = SpawnRigComponent(rhand, rhandRig, handRNM);
        if (headObj == null)
            headObj = SpawnRigComponent(head, headRig, headNM);

        // left hand
        bool res = await SendRigComponent(lHandObj);
        res = await SendRigComponent(rHandObj);
        res = await SendRigComponent(headObj);

        Debug.Log("Spawned");
    }

    private async Task<bool> SendRigComponent(GameObject rigComponent)
    {
        // send/update to server
        bool val = await objCont.ContainsObject(rigComponent.name);
        if (val)
        {
            Destroy(rigComponent);
            // await objCont.DestroyObject(name);
            return false;
        }
        else
        {
            await objCont.AddObjectAsync(rigComponent);
            return true;
        }
    }

    internal void SetBrushScale(Vector3 scale)
    {
        if (rHandObj == null)
            rHandObj = GameObject.Find(handRNM);

        rHandObj.transform.localScale = scale;
    }
}
