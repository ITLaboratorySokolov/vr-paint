using System;
using UnityEngine;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Library
{
    public class ServerSessionAdapterWrapperEventsHandler : MonoBehaviour
    {
        public bool WorldObjectAddedCalled { get; private set; }
        public bool WorldObjectPropertiesUpdatedCalled { get; private set; }
        public bool WorldObjectUpdatedCalled { get; private set; }
        public bool WorldObjectRemovedCalled { get; private set; }
        public bool WorldObjectTransformedCalled { get; private set; }
        
        public void OnWorldObjectAdded(string name)
        {
            try
            {
                Debug.Log("World object added");
                var tempObject = new GameObject();
                Destroy(tempObject);
                WorldObjectAddedCalled = true;                
                Debug.Log("Add finished");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnWorldObjectPropertiesUpdated(string name)
        {
            try
            {
                Debug.Log("World object properties updated");
                var tempObject = new GameObject();
                Destroy(tempObject);
                WorldObjectPropertiesUpdatedCalled = true;
                Debug.Log("Properties update finished");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnWorldObjectUpdated(string name)
        {
            try
            {
                Debug.Log("World object updated");
                var tempObject = new GameObject();
                Destroy(tempObject);
                WorldObjectUpdatedCalled = true;
                Debug.Log("Update finished");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnWorldObjectRemoved(string name)
        {
            try
            {
                Debug.Log("World object removed");
                var tempObject = new GameObject();
                Destroy(tempObject);
                WorldObjectRemovedCalled = true;
                Debug.Log("Removal finished");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void OnWorldObjectTransformed(WorldObjectTransformDto transformDto)
        {
            try
            {
                Debug.Log("World object transformed");
                var tempObject = new GameObject();
                Destroy(tempObject);
                WorldObjectTransformedCalled = true;
                Debug.Log("Transformation finished");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}