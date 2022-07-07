using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers
{
    /// <summary>
    /// Inteface that prescribes how properties of a world object should be managed.
    /// 
    /// Properties setters in this interface should be used only when the world object is changed from a server. 
    /// Because of this, methods in the interface do not affect events.
    /// 
    /// Events are affected only when a property is set directly with Unity classes. For example when a UnityEngine.Mesh
    /// or verticies and triangles as arrays are set to MeshPropertiesManager via <see cref="MeshPropertiesManager.SetMesh(Mesh)"/>,
    /// <see cref="MeshPropertiesManager.SetVertices(Vector3[])"/>, <see cref="MeshPropertiesManager.SetTriangles(int[])"/> and so on.
    /// Triggered events then result in an update which is sent to a server.
    /// </summary>
    public interface IPropertiesManager
    {
        /// <summary>
        /// Event called when some of the properties change.
        /// </summary>
        event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        /// <summary>
        /// Gets a type of a managed world object.
        /// </summary>
        string ManagedType { get; }

        /// <summary>
        /// Gets properties of a world object.
        /// </summary>
        /// <returns>Properties of a world object.</returns>
        Dictionary<string, byte[]> GetProperties();

        /// <summary>
        /// Sets properties of a world object.
        /// 
        /// This method does not cause events <see cref="PropertiesChanged"/> to be called.
        /// </summary>
        /// <param name="properties">Properties of a world object.</param>
        void SetProperties(Dictionary<string, byte[]> properties);
    }
}