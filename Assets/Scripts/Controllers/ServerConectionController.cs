using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

/// <summary>
/// Script used to controll connection to server
/// - handles connecting, disconnecting, reconnecting
/// - processes mesh to be send to server
/// </summary>
public class ServerConectionController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField]
    PaintController paintCont;
    [SerializeField]
    ObjectController objCont;
    [SerializeField]
    RigController rigSpawner;

    [Header("Serializers")]
    RawMeshSerializer serializer;

    [Header("Connection")]
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    /// <summary> Name of client </summary>
    [SerializeField]
    private StringVariable clientName;
    /// <summary> Synchronization call has been finished </summary>
    internal bool syncCallDone;
    /// <summary> Highest number of line on server </summary>
    int serverLines;

    [Header("Actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary> Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        actionStart.Invoke();
        StartCoroutine(InitAndStartSessionCoroutine());
        
        serializer = new RawMeshSerializer();
    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        syncCallDone = false;
        paintCont.ToggleReadyForInput(false);

        Debug.Log("Launching restart procedure");

        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// Starting coroutine
    /// - setting connection
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator InitAndStartSessionCoroutine()
    {
        var task = session.InitializeAsync();

        while (!task.IsCompleted)
            yield return null;

        task = session.StartSessionAsync();

        while (!task.IsCompleted)
            yield return null;
    }

    /// <summary>
    /// On disconnected from server
    /// - clear local objects
    /// - start reconnect procedure
    /// </summary>
    public void OnDisconnected()
    {
        syncCallDone = false;
        paintCont.ToggleReadyForInput(false);
        objCont.ObjectsClear();
        rigSpawner.SpawnRig();

        Debug.Log("Disconnected - Launching restart procedure");

        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// On reconnecting to server
    /// </summary>
    public void OnReconnecting()
    {
        syncCallDone = false;
        paintCont.ToggleReadyForInput(false);
     
        Debug.Log("Lost connection");
    }

    /// <summary>
    /// On reconnected to server
    /// - delete local objects 
    /// - spawn local object again
    /// </summary>
    public void OnReconnected()
    {
        paintCont.ToggleReadyForInput(true);
        objCont.ObjectsClear();

        Debug.Log("Regained connection");
        
        StartCoroutine(SyncCall());
        SpawnLocalObjects();
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a 5s delay between attempts
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartConnection()
    {
        yield return new WaitForSeconds(5);
        actionStart.Invoke();
        /*
        var task = session.InitializeAsync();
        while (!task.IsCompleted)
            yield return null;
        */
        var task = session.StartSessionAsync();
        while (!task.IsCompleted)
            yield return null;
    }

    /// <summary>
    /// Reset connection to server
    /// </summary>
    public void ResetConnection()
    {
        syncCallDone = false;
    }

    /// <summary>
    /// Spawn local objects
    /// </summary>
    public void SpawnLocalObjects()
    {
        StartCoroutine(SpawnRigCorout());
    }

    /// <summary>
    /// Spawn rig and send it to server
    /// </summary>
    IEnumerator SpawnRigCorout()
    {
        yield return new WaitUntil(() => syncCallDone);

        var tr = rigSpawner.AddRigToServer();

        while (!tr.IsCompleted)
            yield return null;

        // set rig movement controlls

        // left hand
        var lhr = GameObject.Find("LeftHand Controller");
        var lhs = GameObject.Find(rigSpawner.handLNM);
        var mc = lhs.AddComponent<MoveController>();
        mc.parent = lhr.transform;

        // right hand
        var rhr = GameObject.Find("RightHand Controller");
        var rhs = GameObject.Find(rigSpawner.handRNM);
        mc = rhs.AddComponent<MoveController>();
        mc.parent = rhr.transform;

        // head
        var hr = GameObject.Find("Main Camera");
        var hs = GameObject.Find(rigSpawner.headNM);
        mc = hs.AddComponent<MoveController>();
        mc.parent = hr.transform;

        rigSpawner.SwapColor(true);
        paintCont.SetBrushWidth();
    }

    /// <summary>
    /// On application exit
    /// </summary>
    internal void OnExit()
    {
        StartCoroutine(ExitCorout());
    }

    /// <summary>
    /// Exit coroutine
    /// - remove rig from server and exit application
    /// </summary>
    private IEnumerator ExitCorout()
    {
        yield return StartCoroutine(RemoveRigCorout());
        Application.Quit();
    }

    /// <summary>
    /// Remove rig from server
    /// </summary>
    IEnumerator RemoveRigCorout()
    {
        var tr = rigSpawner.RemoveRigFromServer();

        while (!tr.IsCompleted)
            yield return null;
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
                if (n.StartsWith("Line_"))
                {
                    int nmlen = ("Line_" + clientName.Value).Length + 1;

                    Debug.Log(n);

                    string num = n.Substring(nmlen);
                    int numP = 0;
                    int.TryParse(num, out numP);

                    Debug.Log(numP);

                    // if the lane was drawn by this client
                    if (n.Equals("Line_" + clientName.Value + "_" + numP))
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
    internal void SendTriangleStripToServer(GameObject obj, Texture2D t2D)
    {
        if (obj == null)
            return;

        // Set properties
        MeshPropertiesManager propsManager = obj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        propsManager.name = obj.name;

        Dictionary<string, byte[]> props;
        if (t2D != null)
            props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle", ConvertorHelper.Vec2ToFloats(mesh.uv), t2D.width, t2D.height, "RGBA", t2D.GetRawTextureData());
        else
            props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");

        propsManager.SetProperties(props);
        propsManager.SetMesh(mesh);

        StartCoroutine(AddObjectCoroutine(obj));
    }

    /// <summary>
    /// Add object to server
    /// </summary>
    /// <param name="obj"> Object to add </param>
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
    internal void UpdateTriangleStripOnServer(GameObject obj, Texture2D t2D)
    {
        if (obj == null)
            return;

        MeshPropertiesManager propsManager = obj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

        Dictionary<string, byte[]> props;
        if (t2D != null)
            props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle", ConvertorHelper.Vec2ToFloats(mesh.uv), t2D.width, t2D.height, "RGBA", t2D.GetRawTextureData());
        else
            props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");

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

    /// <summary>
    /// Destroy object coroutine
    /// </summary>
    /// <param name="name"> Name of object </param>
    /// <param name="obj"> Game object </param>
    IEnumerator DestroyObjectCoroutine(string name, GameObject obj)
    {
        var t = objCont.DestroyObject(name, obj);
        while (!t.IsCompleted)
            yield return null;
    }
}
