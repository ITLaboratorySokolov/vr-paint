using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;

/// <summary>
/// Script controlling displayed rig
/// </summary>
public class RigController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    GameObject head;
    [SerializeField]
    GameObject lhand;
    [SerializeField]
    GameObject rhand;

    [Header("Transforms")]
    [SerializeField]
    Transform headRig;
    [SerializeField]
    Transform lhandRig;
    [SerializeField]
    Transform rhandRig;

    [Header("Controllers")]
    [SerializeField]
    ObjectController objCont;

    [Header("Names")]
    internal string handLNM;
    internal string handRNM;
    internal string headNM;

    [Header("Game objects")]
    GameObject lHandObj;
    GameObject rHandObj;
    GameObject headObj;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartXR());

        // spawn rig components
        SpawnRig();
        SwapColor(false);
    }

    /// <summary>
    /// Start OpenXR
    /// - enables virtual reality on start of the scene
    /// </summary>
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

    /// <summary>
    /// Swap displayed colors of controllers
    /// Color depends on connection status
    /// - red for disconnected
    /// - green for connected
    /// </summary>
    /// <param name="connected"> Connection status </param>
    public void SwapColor(bool connected)
    {
        // find hands in scene
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

    /// <summary>
    /// Spawn headset and controllers
    /// </summary>
    public void SpawnRig()
    {
        handLNM = "HandL_" + objCont.clientName.Value;
        handRNM = "HandR_" + objCont.clientName.Value;
        headNM = "Head_" + objCont.clientName.Value;

        // left hand
        lHandObj = SpawnRigComponent(lhand, lhandRig, handLNM);
        rHandObj = SpawnRigComponent(rhand, rhandRig, handRNM);
        headObj = SpawnRigComponent(head, headRig, headNM);
    }

    /// <summary>
    /// Spawn rig component
    /// </summary>
    /// <param name="prefab"> Prefab of the component </param>
    /// <param name="tfParent"> Parent </param>
    /// <param name="name"> Name of compoment </param>
    /// <returns> Spawned game object </returns>
    private GameObject SpawnRigComponent(GameObject prefab, Transform tfParent, string name)
    {
        Debug.Log("Spawning " + name + " ((search: Removing))");

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

    /// <summary>
    /// Send rig to server
    /// </summary>
    public async Task AddRigToServer()
    {
        handLNM = "HandL_" + objCont.clientName.Value;
        handRNM = "HandR_" + objCont.clientName.Value;
        headNM = "Head_" + objCont.clientName.Value;

        // find / spawn rig components
        if (lHandObj == null)
            lHandObj = SpawnRigComponent(lhand, lhandRig, handLNM);
        if (rHandObj == null)
            rHandObj = SpawnRigComponent(rhand, rhandRig, handRNM);
        if (headObj == null)
            headObj = SpawnRigComponent(head, headRig, headNM);

        bool res = await SendRigComponent(lHandObj);
        res = await SendRigComponent(rHandObj);
        res = await SendRigComponent(headObj);

        Debug.Log("Spawned");
    }

    /// <summary>
    /// Send rig component to server
    /// </summary>
    /// <param name="rigComponent"> Game object to send </param>
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

    /// <summary>
    /// Remove rig objects from server
    /// </summary>
    public async Task RemoveRigFromServer()
    {
        bool res = await RemoveRigComponent(handLNM);
        res = await RemoveRigComponent(handRNM);
        res = await RemoveRigComponent(headNM);

        Debug.Log("Removed from server");
    }

    /// <summary>
    /// Remove rig component from server
    /// </summary>
    /// <param name="name"> Name of the component </param>
    private async Task<bool> RemoveRigComponent(string name)
    {
        bool val = await objCont.ContainsObject(name);
        if (val)
        {
            await objCont.RemoveObject(name);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Set scale of brush
    /// </summary>
    /// <param name="scale"> Scale </param>
    internal void SetBrushScale(Vector3 scale)
    {
        if (rHandObj == null)
            rHandObj = GameObject.Find(handRNM);

        rHandObj.transform.localScale = scale;
    }
}
