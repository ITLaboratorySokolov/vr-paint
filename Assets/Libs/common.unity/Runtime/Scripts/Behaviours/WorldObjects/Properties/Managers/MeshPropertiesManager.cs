using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Models.Utility;
using ZCU.TechnologyLab.Common.Unity.Models.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers
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
    /// If you use the first option, the changes to the mesh will not trigger <see cref="PropertiesChanged"/> 
    /// event and even if you add the mesh to <see cref="WorldObjectManager"/> 
    /// it will not propagate changes to a server. You would have to update the mesh manually by 
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
        private const string ManagedTypeDescription = "Mesh";

        /// <summary>
        /// Supported mesh primitives.
        /// </summary>
        private static readonly string[] SupportedPrimitives = { "Triangle" };

        [SerializeField]
        [FormerlySerializedAs("optionalPropertiesManager")]
        private OptionalPropertiesManager _optionalPropertiesManager;

        /// <summary>
        /// Mesh filter.
        /// </summary>
        /// <remarks>
        /// Provides information about a mesh.
        /// </remarks>
        private MeshFilter _meshFilter;

        /// <summary>
        /// Mesh renderer.
        /// </summary>
        /// <remarks>
        /// Provides information about material of a mesh.
        /// </remarks>
        private MeshRenderer _meshRenderer;

        /// <summary>
        /// Mesh serializer factory.
        /// </summary>
        private MeshSerializerFactory _meshSerializerFactory = new();

        /// <inheritdoc/>
        public string ManagedType => ManagedTypeDescription;

        /// <summary>
        /// Initializes mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            InitializeMesh();
            
            _meshRenderer = GetComponent<MeshRenderer>();
            InitializeMaterial();
        }
        
        /// <inheritdoc/>
        public Dictionary<string, byte[]> GetProperties()
        {
            var properties = _meshSerializerFactory.RawMeshSerializer.Serialize(
                PointConverter.Point3DToFloat(_meshFilter.mesh.vertices), 
                _meshFilter.mesh.triangles, SupportedPrimitives[0]);
            
            _optionalPropertiesManager.AddProperties(properties);
            return properties;
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            if(_meshSerializerFactory.IsRawMesh(properties))
            {
                SetRawMeshProperties(properties);
            } else if (_meshSerializerFactory.IsPlyFile(properties))
            {
                // TODO ply file
            }
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
            var mesh = _meshFilter.mesh;
            mesh.vertices = vertices;
            RecalculateMesh(mesh);

            InvokePropertiesChanged();
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
            var mesh = _meshFilter.mesh;
            mesh.triangles = triangles;
            RecalculateMesh(mesh);

            InvokePropertiesChanged();
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
            UpdateMesh(vertices, triangles);
            InvokePropertiesChanged();
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
            _meshFilter.mesh = mesh;
            InvokePropertiesChanged();
        }

        /// <summary>
        /// Sets a new material.
        /// </summary>
        /// <param name="material">The material.</param>
        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
        
        private void InitializeMaterial()
        {
            if (_meshRenderer.material == null)
            {
                _meshRenderer.material = new Material(Shader.Find("Diffuse"));
            }
        }

        private void InitializeMesh()
        {
            if (_meshFilter.mesh == null)
            {
                _meshFilter.mesh = new Mesh();
            }
        }
        
        private void UpdateMesh(Vector3[] vertices, int[] triangles)
        {
            var mesh = _meshFilter.mesh;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            RecalculateMesh(mesh);
        }
        
        private static void RecalculateMesh(Mesh mesh)
        {
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        private void InvokePropertiesChanged()
        {
            PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = gameObject.name,
                Properties = GetProperties()
            });
        }
        
        private void SetRawMeshProperties(Dictionary<string, byte[]> properties)
        {
            var meshSerializer = _meshSerializerFactory.RawMeshSerializer;
            if (!SupportedPrimitives.Contains(meshSerializer.PrimitiveSerializer.Deserialize(properties)))
            {
                return;
            }

            var vertices = PointConverter.FloatToPoint3D(meshSerializer.VerticesSerializer.Deserialize(properties));
            var triangles = meshSerializer.IndicesSerializer.Deserialize(properties);
            UpdateMesh(vertices, triangles);

            _optionalPropertiesManager.SetProperties(properties);
        }
    }
}