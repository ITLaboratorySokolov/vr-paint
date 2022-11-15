using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.WorldObjects;

public class ObjectController : MonoBehaviour
{
    public WorldObjectManager wom;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Task<IEnumerable<GameObject>> ObjectRecieve()
    {
        Debug.Log("Recieve all");
        return wom.LoadServerContentAsync();
    }

    public void ObjectRemoved()
    {
        Debug.Log("Remove");
    }

    public void ObjectsClear()
    {
        Debug.Log("Clear");
    }

    public void ObjectsReplace()
    {
        Debug.Log("Replace");
    }

    internal async void DestroyObject(string name)
    {
        Debug.Log("Destroy " + name);
        await wom.RemoveObjectAsync(name);

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
