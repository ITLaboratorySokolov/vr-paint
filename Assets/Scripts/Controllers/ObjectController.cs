using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

public class ObjectController : MonoBehaviour
{
    public WorldObjectManager woManager;
    public WorldObjectMemoryStorage woMemoryStorage;
    public ServerDataAdapterWrapper dataAdapter;
    public StringVariable clientName;

    public Task<IEnumerable<GameObject>> ObjectRecieve()
    {
        Debug.Log("Recieve all");
        return woManager.LoadServerContentAsync();
    }

    public async Task ObjectRemoved(string name)
    {
        Debug.Log("Remove");
        await woManager.RemoveObjectAsync(name);
    }

    public void ObjectsClear()
    {
        Debug.Log("Clear");
        woManager.ClearLocalContent();
    }

    public void RemoveObjectFromLocal(string name)
    {
        Debug.Log("Remove local");
        GameObject o;
        bool res = woMemoryStorage.Remove(name, out o);
        if (res)
            Destroy(o);
    }

    public async Task DestroyObject(string name, GameObject obj)
    {
        Debug.Log("Destroy " + name);
        await woManager.RemoveObjectAsync(name);
        Destroy(obj);
        Debug.Log("Destroyed " + name);
    }

    public async Task AddObjectAsync(GameObject obj)
    {
        Debug.Log("Add " + obj.name);
        await woManager.AddObjectAsync(obj);

    }

    public async Task UpdateProperties(string name)
    {
        Debug.Log("Update " + name);
        await woManager.UpdateObjectAsync(name);
    }

    public async Task<bool> ContainsObject(string name)
    {
        return await dataAdapter.ContainsWorldObjectAsync(name);
    }

    public async Task RefreshObject(string name)
    {
        await dataAdapter.GetAllWorldObjectsAsync();
        //var d = await dataAdapter.GetWorldObjectAsync(name);
        Debug.Log("Refreshed " + name);
    }
}
