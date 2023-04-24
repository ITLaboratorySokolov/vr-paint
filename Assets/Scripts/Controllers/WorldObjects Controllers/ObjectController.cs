using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

/// <summary>
/// Script used for sending, deleting or updating objects on server
/// </summary>
public class ObjectController : MonoBehaviour
{
    [SerializeField]
    WorldObjectManager woManager;
    [SerializeField]
    WorldObjectMemoryStorage woMemoryStorage;
    [SerializeField]
    ServerDataAdapterWrapper dataAdapter;
    [SerializeField]
    public StringVariable clientName;

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
