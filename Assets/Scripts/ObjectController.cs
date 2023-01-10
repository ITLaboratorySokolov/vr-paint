using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;

public class ObjectController : MonoBehaviour
{
    public WorldObjectManager wom;

    public Task<IEnumerable<GameObject>> ObjectRecieve()
    {
        Debug.Log("Recieve all");
        return wom.LoadServerContentAsync();
    }

    public async void ObjectRemoved(string name)
    {
        Debug.Log("Remove");
        await wom.RemoveObjectAsync(name);
    }

    public void ObjectsClear()
    {
        Debug.Log("Clear");
        wom.ClearLocalContent();
    }

    internal async void DestroyObject(string name, GameObject obj)
    {
        Debug.Log("Destroy " + name);
        await wom.RemoveObjectAsync(name);
        Destroy(obj);
        Debug.Log("Destroyed " + name);
    }

    internal async void AddObjectAsync(GameObject obj)
    {
        Debug.Log("Add " + obj.name);
        await wom.AddObjectAsync(obj);

    }

    internal async void UpdateProperties(string name)
    {
        Debug.Log("Update " + name);
        await wom.UpdateObjectAsync(name);
    }
}
