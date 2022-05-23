using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.Utility;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties
{
    /// <summary>
    /// The <see cref="MeshPropertiesManager"/> class provides access to properties of a mesh
    /// and reports when some of the properties are changed.
    /// 
    /// The class requires that <see cref="MeshFilter"/> and <see cref="MeshRenderer"/> are added to the game object.
    /// If they are added before <see cref="MeshPropertiesManager"/>, the manager works with the mesh
    /// assigned to <see cref="MeshFilter"/> and material assigned to <see cref="MeshRenderer"/>.
    /// Otherwise, when these classes are not on the same game object, the manager
    /// creates its own mesh and material and creates new <see cref="MeshFilter"/> and <see cref="MeshRenderer"/>.
    /// 
    /// If you need to change the mesh you can do it in multiple ways:
    ///     1) Via your own custom classes
    ///     2) Via <see cref="SetMesh"/>, <see cref="SetVertices"/>, <see cref="SetTriangles"/>, <see cref="SetVerticesAndTriangles"/>
    ///     3) Via <see cref="SetProperties"/>
    /// 
    /// If you use the first option, the changes to the mesh will not triggger <see cref="PropertiesChanged"/> 
    /// event and even if you add the mesh to <see cref="WorldObjectManager"/> 
    /// it will not propagete changes to a server. You would have to update the mesh manually by 
    /// <see cref="WorldObjectManager.UpdateObjectAsync"/>.
    /// 
    /// If you want to propagate changes via events automatically you can use the second option, but 
    /// the mesh should be added to <see cref="WorldObjectManager"/> to actually send the updates to the server.
    /// Please keep in mind that all these methods except the first one recalculate bounds and normals
    /// to keep the mesh consistent.
    /// 
    /// The third option is not supposed to update properties by a user. 
    /// It should be used exclusively for communication between the application and the server.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshPropertiesManager : MonoBehaviour, IPropertiesManager
    {
        /// <inheritdoc/>
        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;

        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        public const string ManagedTypeDescription = "Mesh";

        /// <summary>
        /// Supported mesh primitives.
        /// </summary>
        private static readonly string[] SupportedPrimitives = { "Triangle" };

        /// <summary>
        /// Mesh filter.
        /// </summary>
        /// <remarks>
        /// Provides informations about a mesh.
        /// </remarks>
        private MeshFilter meshFilter;

        /// <summary>
        /// Mesh renderer.
        /// </summary>
        /// <remarks>
        /// Provides informations about material of a mesh.
        /// </remarks>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Mesh serializer.
        /// </summary>
        private MeshSerializer meshSerializer;

        /// <inheritdoc/>
        public string ManagedType => ManagedTypeDescription;

        /// <summary>
        /// Initializes mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            this.meshFilter = GetComponent<MeshFilter>();
            if(this.meshFilter.mesh == null)
            {
                this.meshFilter.mesh = new Mesh();
            }
            
            this.meshRenderer = GetComponent<MeshRenderer>();
            if(this.meshRenderer.material == null)
            {
                this.meshRenderer.material = new Material(Shader.Find("Diffuse"));
            }

            this.meshSerializer = new MeshSerializer();
        }

        /// <summary>
        /// Sets a new mesh.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When a mesh of a MeshFilter is changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void SetMesh(Mesh mesh)
        {
            this.meshFilter.mesh = mesh;
            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
        }

        /// <summary>
        /// Sets a new material.
        /// </summary>
        /// <param name="material">The material.</param>
        public void SetMaterial(Material material)
        {
             this.meshRenderer.material = material;
        }

        /// <summary>
        /// Sets vertices and triangles of a mesh.
        /// 
        /// The method recalculates normals and bounds of the mesh.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When vertices or triangles of a mesh are changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="vertices">Vertices.</param>
        /// <param name="triangles">Triangles.</param>
        public void SetVerticesAndTriangles(Vector3[] vertices, int[] triangles)
        {
            this.meshFilter.mesh.vertices = vertices;
            this.meshFilter.mesh.triangles = triangles;
            this.meshFilter.mesh.RecalculateNormals();
            this.meshFilter.mesh.RecalculateBounds();

            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
        }

        /// <summary>
        /// Sets vertices of a mesh.
        /// 
        /// The method recalculates normals and bounds of the mesh.
        /// If you need to set triangles as well, use <see cref="SetVerticesAndTriangles(Vector3[], int[])"/>
        /// to save one recalculation of normals and bounds.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When vertices of a mesh are changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="vertices">Vertices.</param>
        public void SetVertices(Vector3[] vertices)
        {
            this.meshFilter.mesh.vertices = vertices;
            this.meshFilter.mesh.RecalculateNormals();
            this.meshFilter.mesh.RecalculateBounds();

            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            { 
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
        }
        /// <summary>
        /// Sets triangles of a mesh.
        /// 
        /// The method recalculates normals and bounds of the mesh.
        /// If you need to set vertices as well, use <see cref="SetVerticesAndTriangles(Vector3[], int[])"/>
        /// to save one recalculation of normals and bounds.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When triangles of a mesh are changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="triangles">Triangles.</param>
        public void SetTriangles(int[] triangles)
        {
            this.meshFilter.mesh.triangles = triangles;
            this.meshFilter.mesh.RecalculateNormals();
            this.meshFilter.mesh.RecalculateBounds();

            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
        }

        /// <inheritdoc/>
        public Dictionary<string, byte[]> GetProperties()
        {
            return this.meshSerializer.SerializeProperties(PointConverter.Point3DToFloat(this.meshFilter.mesh.vertices), this.meshFilter.mesh.triangles, SupportedPrimitives[0]);
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            if (this.meshSerializer.SupportPrimitive(properties, SupportedPrimitives))
            {
                this.meshFilter.mesh.vertices = PointConverter.FloatToPoint3D(this.meshSerializer.DeserializeVertices(properties));
                this.meshFilter.mesh.triangles = this.meshSerializer.DeserializeIndices(properties);
                this.meshFilter.mesh.RecalculateNormals();
                this.meshFilter.mesh.RecalculateBounds();
            }
        }
    }
}
