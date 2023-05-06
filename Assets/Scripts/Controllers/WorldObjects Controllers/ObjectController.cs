using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;

/// <summary>
/// Script used for sending, deleting or updating objects on server
/// </summary>
public class ObjectController : MonoBehaviour
{
    [SerializeField]
    WorldObjectManager woManager;
    [SerializeField]
    WorldObjectMemoryStorageWrapper woMemoryStorage;
    [SerializeField]
    ServerDataAdapterWrapper dataAdapter;
    [SerializeField]
    public StringVariable clientName;

    /// <summary>
    /// Get all server objects currently stored in scene
    /// </summary>
    /// <returns> Game objects </returns>
    public IEnumerable<GameObject> GetStoredObjects()
    {
        if (woMemoryStorage == null)
            Debug.LogError("wtf");

        return woMemoryStorage.GetAll();
    }

    /// <summary>
    /// Clear all objects from server
    /// </summary>
    /// <returns> True if executed successfully </returns>
    public async Task<bool> ClearObjectsFromServer()
    {
        IEnumerable<GameObject> objs = GetStoredObjects();
        List<GameObject> objsList = new List<GameObject>();
        foreach (GameObject o in objs)
            if (!(o.name.StartsWith("HandL_") || o.name.StartsWith("HandR_") || o.name.StartsWith("Head_")))
                objsList.Add(o);
        await RemoveObjects(objsList);

        return true;
    }

    /// <summary>
    /// Remove multiple objects from server and local
    /// </summary>
    /// <param name="objs"> List of obects </param>
    public async Task RemoveObjects(List<GameObject> objs)
    {
        for (int i = 0; i < objs.Count; i++)
        {
            Debug.Log(objs[i].name);
            await DestroyObject(objs[i].name, objs[i]);
        }
    }

    /// <summary>
    /// Load server content
    /// </summary>
    public Task<IEnumerable<GameObject>> ObjectRecieve()
    {
        Debug.Log("Recieve all");
        return woManager.LoadServerContentAsync();
    }

    /// <summary>
    /// Remove object from server
    /// </summary>
    /// <param name="name"> Name of object </param>
    public async Task RemoveObject(string name)
    {
        Debug.Log("Remove");
        await woManager.RemoveObjectAsync(name);
    }

    /// <summary>
    /// Clear local content
    /// </summary>
    public void ObjectsClear()
    {
        Debug.Log("Clear");
        woManager.ClearLocalContent();
    }

    /// <summary>
    /// Remove object from server and local
    /// </summary>
    /// <param name="name"> Name of object </param>
    /// <param name="obj"> Game object </param>
    public async Task DestroyObject(string name, GameObject obj)
    {
        Debug.Log("Destroy " + name);
        await woManager.RemoveObjectAsync(name);
        Destroy(obj);
        Debug.Log("Destroyed " + name);
    }

    /// <summary>
    /// Add object to server
    /// </summary>
    /// <param name="obj"> Game object </param>
    public async Task AddObjectAsync(GameObject obj)
    {
        Debug.Log("Add " + obj.name);
        await woManager.AddObjectAsync(obj);
    }

    /// <summary>
    /// Update properties of object
    /// </summary>
    /// <param name="name"> Name of object </param>
    /// <returns></returns>
    public async Task UpdateProperties(string name)
    {
        Debug.Log("Update " + name);
        await woManager.UpdateObjectAsync(name);
    }

    /// <summary>
    /// Does server contain object with name
    /// </summary>
    /// <param name="name"> Name of object </param>
    /// <returns></returns>
    public async Task<bool> ContainsObject(string name)
    {
        return await dataAdapter.ContainsWorldObjectAsync(name);
    }

    /// <summary>
    /// New object was added at runtime
    /// </summary>
    /// <param name="o"></param>
    public void AddedNewObjectAtRuntime(GameObject o)
    {
        var iph = o.GetComponent<ObjectPropertiesHandler>();
        if (iph != null)
            iph.objCont = this;
    }
}
