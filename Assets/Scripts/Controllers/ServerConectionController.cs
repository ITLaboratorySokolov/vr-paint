using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

public class ServerConectionController : MonoBehaviour
{
    [Header("Controllers")]
    /// <summary> Paint controller </summary>
    [SerializeField]
    PaintController paintCont;
    [SerializeField]
    ObjectController objCont;

    [Header("Serializers")]
    /// <summary> Mesh serializer </summary>
    RawMeshSerializer serializer;

    [Header("Connection")]
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;

    WorldObjectManager wom;


    /// <summary> Synchronization call has been finished </summary>
    internal bool syncCallDone;
    /// <summary> Highest number of line on server </summary>
    int serverLines;
    /// <summary> Name of client </summary>
    [SerializeField]
    private StringVariable clientName;

    [Header("Actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        actionStart.Invoke();
        serializer = new RawMeshSerializer();
    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        paintCont.ToggleReadyForInput(false);
        Debug.Log("Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    public void OnDisconnected()
    {
        paintCont.ToggleReadyForInput(false);
        objCont.ObjectsClear();
        Debug.Log("Disconnected - Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    public void OnReconnecting()
    {
        Debug.Log("Lost connection");
        paintCont.ToggleReadyForInput(false);
    }

    public void OnReconnected()
    {
        Debug.Log("Regained connection");
        paintCont.ToggleReadyForInput(true);
        // TODO tady by se mìly stáhnout updaty ze serveru
        objCont.ObjectsClear();
        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a minimum 5s delay
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartConnection()
    {
        yield return new WaitForSeconds(5);
        actionStart.Invoke();
    }

    /// <summary>
    /// Reset connection to server
    /// </summary>
    public void ResetConnection()
    {
        syncCallDone = false;
    }

    /// <summary>
    /// Called when successfully connected to server
    /// </summary>
    public void ConnectedToServer()
    {
        Debug.Log("Connected to server");
        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.State == SessionState.Connected);


        
        var res = GetObjectsAsync();

        while (!res.IsCompleted)
            yield return null;

        if (res.Result)
            Debug.Log("Sync call completed");
        else 
            Debug.Log("Sync call unsuccessfull");

    }

    /// <summary>
    /// Get objects from server
    /// - filter lines and display them
    /// </summary>
    private async Task<bool> GetObjectsAsync()
    {
        bool res = true;

        try
        {
            // Get all objects
            IEnumerable<GameObject> gmobjs = await objCont.ObjectRecieve();
            List<int> l = new List<int>();

            // If object is recognized as a line
            serverLines = 0;
            foreach (GameObject obj in gmobjs)
            {
                string n = obj.name;
                Debug.Log(n);

                // Filter out lines
                if (n.StartsWith("Line"))
                {
                    string num = n.Substring(5, n.Length - 5);
                    int numP = 0;
                    int.TryParse(num, out numP);

                    // if the lane was drawn by this client
                    string cName = n.Substring(4, 1);
                    if (cName.Equals(clientName.Value))
                        l.Add(numP);
                 
                    paintCont.AddServerLine(obj);
                }
             
            }

            serverLines = -1;
            for (int i = 0; i < l.Count; i++)
                if (l[i] > serverLines)
                    serverLines = l[i];

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            res = false;
        }

        syncCallDone = true;
        paintCont.ToggleReadyForInput(true, serverLines);
        return res;
    }

    /// <summary>
    /// Action called on ending the application
    /// </summary>
    public void OnDestroy()
    {
        actionEnd.Invoke();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Send triangle strip mesh to server
    /// </summary>
    /// <param name="obj"> Game object </param>
    internal void SendTriangleStripToServer(GameObject obj)
    {
        // Set properties
        MeshPropertiesManager propsManager = obj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        propsManager.name = obj.name;

        // TODO this doesnt send UVS?
        Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle", "_MainTex", ConvertorHelper.Vec2ToFloats(mesh.uv));
        propsManager.SetProperties(props);
        propsManager.SetMesh(mesh);

        StartCoroutine(AddObjectCoroutine(obj));
    }

    IEnumerator AddObjectCoroutine(GameObject obj)
    {
        var t = objCont.AddObjectAsync(obj);
        while (!t.IsCompleted)
            yield return null;

    }

    /// <summary>
    /// Update triangle strip mesh on server
    /// </summary>
    /// <param name="obj"> Game object </param>
    internal void UpdateTriangleStripOnServer(GameObject obj)
    {
        MeshPropertiesManager propsManager = obj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

        Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle", "_MainTex", ConvertorHelper.Vec2ToFloats(mesh.uv));
        // Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");
        propsManager.SetMesh(mesh);
        propsManager.SetProperties(props);
    }


    /// <summary>
    /// Remove object from server and scene
    /// </summary>
    /// <param name="name"> Object name </param>
    /// <param name="obj"> Game object </param>
    internal void DestroyObjectOnServer(string name, GameObject obj)
    {
        StartCoroutine(DestroyObjectCoroutine(name, obj));

    }

    IEnumerator DestroyObjectCoroutine(string name, GameObject obj)
    {
        var t = objCont.DestroyObject(name, obj);
        while (!t.IsCompleted)
            yield return null;
    }

}
