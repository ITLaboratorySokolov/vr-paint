using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Models.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers
{
    /// <summary>
    /// The <see cref="BitmapPropertiesManager"/> class provides access to properties of an image
    /// and reports when some of the properties are changed.
    /// 
    /// The class requires that <see cref="MeshFilter"/> and <see cref="MeshRenderer"/> are added to the game object.
    /// If they are added before <see cref="BitmapPropertiesManager"/>, manager works with the mesh
    /// assigned to <see cref="MeshFilter"/> and material assigned to <see cref="MeshRenderer"/>.
    /// Otherwise, when these classes are not on the same game object, the manager
    /// creates its own mesh and material and creates new <see cref="MeshFilter"/> and <see cref="MeshRenderer"/>.
    /// 
    /// If you need to change the image you can do it in multiple ways:
    ///     1) Via your own custom classes
    ///     2) Via <see cref="SetTexture"/>, <see cref="SetTextureData"/>
    ///     3) Via <see cref="SetProperties"/>/>
    /// 
    /// If you use the first option, the changes to the image will not trigger <see cref="PropertiesChanged"/>
    /// event and even if you add the image to <see cref="WorldObjectManager"/> 
    /// it will not propagate changes to a server. You would have to update the image manually by 
    /// <see cref="WorldObjectManager.UpdateObjectAsync"/>.
    /// 
    /// If you want to propagate changes via events automatically you can use the second option, but 
    /// the image should be added to <see cref="WorldObjectManager"/> to actually send the updates to the server.
    /// 
    /// The third option is not supposed to update properties by a user. 
    /// It should be used exclusively for communication between the application and the server.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BitmapPropertiesManager : MonoBehaviour, IPropertiesManager
    {
        /// <inheritdoc/>
        public event EventHandler<PropertiesChangedEventArgs> PropertiesChanged;
        
        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        private const string ManagedTypeDescription = "Bitmap";

        [SerializeField]
        [FormerlySerializedAs("supportedPixelFormats")]
        private List<PixelFormat> _supportedPixelFormats = new()
        {
            new PixelFormat { Name = "RGB", Format = TextureFormat.RGB24 },
            new PixelFormat { Name = "RGBA", Format = TextureFormat.RGBA32 },
            new PixelFormat { Name = "ARGB", Format = TextureFormat.ARGB32 }
        };
        
        [SerializeField] 
        [FormerlySerializedAs("optionalPropertiesManager")]
        private OptionalPropertiesManager _optionalPropertiesManager;

        /// <summary>
        /// Mesh filter.
        /// </summary>
        /// <remarks>
        /// Provides informations about a mesh.
        /// </remarks>
        private MeshFilter _meshFilter;

        /// <summary>
        /// Mesh renderer.
        /// </summary>
        /// <remarks>
        /// Provides informations about material of a mesh.
        /// </remarks>
        private MeshRenderer _meshRenderer;

        private TextureManipulation _textureManipulation;

        /// <inheritdoc/>
        public string ManagedType => ManagedTypeDescription;

        /// <summary>
        /// Initializes texture, mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _textureManipulation = new TextureManipulation(_supportedPixelFormats);
        }

        /// <summary>
        /// Sets a new texture.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When a texture of a material is changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Old texture.</returns>
        public Texture SetTexture(Texture2D texture)
        {
            var material = _meshRenderer.material;
            var mesh = _meshFilter.mesh;
            
            var oldTexture = _textureManipulation.SetTextureWithSupportedFormat(texture, material);
            MeshManipulation.UpdateMeshToSize(mesh, texture.width, texture.height);
            
            PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = gameObject.name,
                Properties = GetProperties()
            });

            return oldTexture;
        }

        /// <summary>
        /// Sets new data of a texture.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When a texture of a material is changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="width">Width of a texture.</param>
        /// <param name="height">Height of a texture.</param>
        /// <param name="format">Pixel format of a texture.</param>
        /// <param name="data">Pixel data of a texture.</param>
        public void SetTextureData(int width, int height, TextureFormat format, byte[] data)
        {
            var material = _meshRenderer.material;
            var mesh = _meshFilter.mesh;

            _textureManipulation.SetTextureDataToMaterial(width, height, format, data, material);
            MeshManipulation.UpdateMeshToSize(mesh, width, height);
            
            PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = gameObject.name,
                Properties = GetProperties()
            });
        }

        /// <inheritdoc/>
        public Dictionary<string, byte[]> GetProperties()
        {
            var material = _meshRenderer.material;

            var properties = _textureManipulation.GetTextureProperties(material);
            
            if (_optionalPropertiesManager != null)
            {
                _optionalPropertiesManager.AddProperties(properties);
            }

            return properties;
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            var material = _meshRenderer.material;
            var mesh = _meshFilter.mesh;
            
            _textureManipulation.SetTexturePropertiesToMaterial(properties, material);

            var texture = material.mainTexture;
            MeshManipulation.UpdateMeshToSize(mesh, texture.width, texture.height);
            
            if (_optionalPropertiesManager != null)
            {
                _optionalPropertiesManager.SetProperties(properties);
            }
        }
    }
}