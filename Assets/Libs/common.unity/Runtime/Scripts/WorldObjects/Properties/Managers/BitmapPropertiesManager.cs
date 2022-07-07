using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Bitmap;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Serializers;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers
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
    /// If you use the first option, the changes to the image will not triggger <see cref="PropertiesChanged"/>
    /// event and even if you add the image to <see cref="WorldObjectManager"/> 
    /// it will not propagete changes to a server. You would have to update the image manually by 
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
        /// RGBA pixel format.
        /// </summary>
        private const string RGBA = "RGBA";

        /// <summary>
        /// RGB pixel format.
        /// </summary>
        private const string RGB = "RGB";

        /// <summary>
        /// ARGB pixel format.
        /// </summary>
        private const string ARGB = "ARGB";

        /// <summary>
        /// Description of a type of this world object.
        /// </summary>
        private const string ManagedTypeDescription = "Bitmap";

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
        /// Bitmap serializer factory.
        /// </summary>
        private BitmapSerializerFactory bitmapSerializerFactory = new BitmapSerializerFactory();

        [SerializeField]
        private List<OptionalProperty> optionalProperties = new List<OptionalProperty>();

        /// <inheritdoc/>
        public string ManagedType => ManagedTypeDescription;

        /// <summary>
        /// Gets list of optional properties.
        /// </summary>
        public IList<OptionalProperty> OptionalProperties => this.optionalProperties;

        /// <summary>
        /// Initializes texture, mesh filter and mesh renderer.
        /// </summary>
        private void Awake()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
            if (this.meshRenderer.material == null)
            {
                this.meshRenderer.material = new Material(Shader.Find("Diffuse"));
            }

            if (this.meshRenderer.material.mainTexture == null)
            {
                this.meshRenderer.material.mainTexture = new Texture2D(1, 1);
            }
            else if (!(this.meshRenderer.material.mainTexture is Texture2D))
            {
                throw new NotSupportedException("Type of texture is not supported. Bitmap supports unly Texture2D");
            }

            this.meshFilter = this.GetComponent<MeshFilter>();
            if(this.meshFilter.mesh == null)
            {
                this.meshFilter.mesh = this.GeneratePlane(1, 1);
            }
        }

        /// <summary>
        /// Sets a new texture.
        /// 
        /// The method triggers <see cref="PropertiesChanged"/> event.
        /// When a texture of a material is changed outside of the scope of this method
        /// the event is not called.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public void SetTexture(Texture2D texture)
        {
            this.meshRenderer.material.mainTexture = texture;
            this.meshFilter.mesh = this.GeneratePlane(texture.width, texture.height);

            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
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
            var texture = (Texture2D)this.meshRenderer.material.mainTexture;
            texture.Reinitialize(width, height, format, false);
            texture.SetPixelData(data, 0);
            texture.Apply();

            this.meshFilter.mesh = GeneratePlane(width, height);

            this.PropertiesChanged?.Invoke(this, new PropertiesChangedEventArgs
            {
                ObjectName = this.gameObject.name,
                Properties = this.GetProperties()
            });
        }

        /// <inheritdoc/>
        public Dictionary<string, byte[]> GetProperties()
        {
            var texture = (Texture2D)this.meshRenderer.material.mainTexture;

            var properties = this.bitmapSerializerFactory.RawBitmapSerializer.Serialize(
                this.meshRenderer.material.mainTexture.width,
                this.meshRenderer.material.mainTexture.height, 
                this.ConvertFormatToString(texture.format),
                texture.GetRawTextureData());

            foreach(var optionalProperty in this.optionalProperties)
            {
                try
                {
                    properties.Add(optionalProperty.GetPropertyName(), optionalProperty.Serialize());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Unable to serialize optional property: {ex.Message}");
                }
            }

            return properties;
        }

        /// <inheritdoc/>
        public void SetProperties(Dictionary<string, byte[]> properties)
        {
            if(this.bitmapSerializerFactory.IsRawBitmap(properties))
            {
                var rawBitmapSerializer = this.bitmapSerializerFactory.RawBitmapSerializer;

                var height = rawBitmapSerializer.HeightSerializer.Deserialize(properties);
                var width = rawBitmapSerializer.WidthSerializer.Deserialize(properties);
                var format = rawBitmapSerializer.FormatSerializer.Deserialize(properties);

                var texture = (Texture2D)this.meshRenderer.material.mainTexture;
                texture.Reinitialize(width, height, this.ConvertFormatFromString(format), false);
                texture.SetPixelData(rawBitmapSerializer.PixelsSerializer.Deserialize(properties), 0);
                texture.Apply();

                this.meshFilter.mesh = GeneratePlane(width, height);

                foreach(var optionalProperty in this.optionalProperties)
                {
                    try
                    { 
                        optionalProperty.Process(properties);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Unable to process optional property: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Generates a mesh of a plane.
        /// </summary>
        /// <returns>The mesh.</returns>
        private Mesh GeneratePlane(int width, int height)
        {
            var maxSize = width > height ? new Vector2(1, (float)height / width) : new Vector2((float)width / height, 1);

            var mesh = new Mesh();
            mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, maxSize.y, 0), new Vector3(maxSize.x, maxSize.y, 0), new Vector3(maxSize.x, 0, 0) };
            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Converts a pixel format from a string to TextureFormat.
        /// </summary>
        /// <param name="format">The pixel format.</param>
        /// <returns>The texture format.</returns>
        /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
        private TextureFormat ConvertFormatFromString(string format)
        {
            switch (format)
            {
                case RGBA: return TextureFormat.RGBA32;
                case ARGB: return TextureFormat.ARGB32;
                case RGB: return TextureFormat.RGB24;
                default: throw new ArgumentException($"Unsupported texture format: {format}"); 
            }
        }

        /// <summary>
        /// Converts a pixel format from TextureFormat to a string.
        /// </summary>
        /// <param name="format">The texture format.</param>
        /// <returns>The pixel fromat in a string.</returns>
        /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
        private string ConvertFormatToString(TextureFormat format)
        {
            switch(format)
            {
                case TextureFormat.RGBA32: return RGBA;
                case TextureFormat.ARGB32: return ARGB;
                case TextureFormat.RGB24: return RGB;
                default: throw new ArgumentException($"Unsupported texture format: {format}");
            }
        }
    }
}
