using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

public class SceneSaveController : MonoBehaviour
{
    [Header("Importer/Exporter")]
    SceneImporter sceneIn;
    SceneExporter sceneOut;
    
    [Header("Server object prefabs")]
    [SerializeField]
    GameObject meshPrefab;
    [SerializeField]
    GameObject bitmapPrefab;

    [Header("Controllers")]
    [SerializeField]
    ObjectController objCont;
    [SerializeField]
    PaintController paintCont;

    // Start is called before the first frame update
    void Start()
    {
        sceneIn = new SceneImporter();
        sceneOut = new SceneExporter();
    }

    /// <summary>
    /// Export scene to a save slot
    /// </summary>
    /// <param name="slot"> Number of save slot </param>
    public void ExportScene(int slot)
    {
        string path = Application.dataPath + "/Saves/" + slot + "/";
        Debug.Log("Saving to " + path);
        
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        IEnumerable<GameObject> serverObjs = objCont.GetStoredObjects();
        List<GameObject> serverObjsList= new List<GameObject>();
        foreach (GameObject o in serverObjs)
            if (!(o.name.StartsWith("HandL_") || o.name.StartsWith("HandR_") || o.name.StartsWith("Head_")))
                serverObjsList.Add(o);

        sceneOut.SaveObjectsToFolder(path, serverObjsList);
    }

    /// <summary>
    /// Import scene from a save slot
    /// </summary>
    /// <param name="slot"> Number of save slot </param>
    public void ImportScene(int slot)
    {
        StartCoroutine(ImportSceneCorout(slot));
    }

    /// <summary>
    /// Import scene from a save slot coroutine
    /// </summary>
    /// <param name="slot"> Number of save slot </param>
    private IEnumerator ImportSceneCorout(int slot)
    {
        // delete all server and local objects
        Debug.Log("Delete server objects");
        Task<bool> t = objCont.ClearObjectsFromServer();
        while (!t.IsCompleted)
            yield return null;

        string path = Application.dataPath + "/Saves/" + slot + "/";
        Debug.Log("Importing from " + path);

        if (!Directory.Exists(path))
        {
            Debug.Log("No save file");
        }
        else
        {
            List<WorldObjectDto> objs = sceneIn.LoadObjectsFromFolder(path);

            // instantiate object
            foreach (WorldObjectDto o in objs)
            {
                // test if object exists on server
                Task<bool> tC = objCont.ContainsObject(o.Name);
                while (!tC.IsCompleted)
                    yield return null;

                // add to server if not there
                if (!tC.Result)
                {
                    GameObject inst = null;
                    // instantiate correct prefab and fill w/ data from world object dto
                    if (o.Type == "mesh")
                    {
                        inst = Instantiate(meshPrefab);
                        inst.name = o.Name;
                        inst.transform.position = VectorDTOToVector3(o.Position);
                        inst.transform.eulerAngles = VectorDTOToVector3(o.Rotation);
                        inst.transform.localScale = VectorDTOToVector3(o.Scale);
                        inst.GetComponent<MeshPropertiesManager>().SetProperties(o.Properties);

                    }
                    else if (o.Type == "bitmap")
                    {
                        inst = Instantiate(bitmapPrefab);
                        inst.name = o.Name;
                        inst.transform.position = VectorDTOToVector3(o.Position);
                        inst.transform.eulerAngles = VectorDTOToVector3(o.Rotation);
                        inst.transform.localScale = VectorDTOToVector3(o.Scale);
                        inst.GetComponent<BitmapPropertiesManager>().SetProperties(o.Properties);
                    }

                    Task t2 = objCont.AddObjectAsync(inst);
                    while (!t2.IsCompleted)
                        yield return null;
                }
                // move the one in scene if already on server
                else
                {
                    GameObject inst = GameObject.Find(o.Name);
                    if (inst != null)
                    {
                        inst.transform.position = VectorDTOToVector3(o.Position);
                        inst.transform.eulerAngles = VectorDTOToVector3(o.Rotation);
                        inst.transform.localScale = VectorDTOToVector3(o.Scale);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Parse RemoteVectorDto to Vector3
    /// </summary>
    /// <param name="vec"> RemoteVectorDto </param>
    /// <returns> Vector3 </returns>
    Vector3 VectorDTOToVector3(RemoteVectorDto vec)
    {
        Vector3 res = new Vector3(vec.X, vec.Y, vec.Z);
        return res;
    }
}
