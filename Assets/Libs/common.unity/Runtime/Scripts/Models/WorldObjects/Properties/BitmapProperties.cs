using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Bitmap;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties
{
    public class BitmapProperties
    {
        private readonly Dictionary<string, TextureFormat> _nameToTextureFormat = new();
        private readonly Dictionary<TextureFormat, string> _textureFormatToName = new(); 
        private readonly BitmapSerializerFactory _bitmapSerializerFactory = new();

        public BitmapProperties(List<PixelFormat> supportedPixelFormats)
        {
            foreach (var pixelFormat in supportedPixelFormats)
            {
                _nameToTextureFormat.Add(pixelFormat.Name, pixelFormat.Format);
                _textureFormatToName.Add(pixelFormat.Format, pixelFormat.Name);
            }
        }

        public Texture SetTexture(Texture2D newTexture, Material material, Mesh mesh)
        {
            AssertSupportedTextureFormat(newTexture.format);
            
            UpdateMesh(mesh, newTexture.width, newTexture.height);
            
            var oldTexture = material.mainTexture;
            material.mainTexture = newTexture;
            return oldTexture;
        }

        public void SetTextureData(int width, int height, TextureFormat format, byte[] data, Material material,
            Mesh mesh)
        {
            AssertSupportedTextureFormat(format);

            UpdateMesh(mesh, width, height);
            
            var texture = material.mainTexture;
            if (texture == null)
            {
                var texture2D = CreateTexture(width, height, format, data);
                material.mainTexture = texture2D;
            }
            else
            {
                var texture2D = ToTexture2D(texture);
                UpdateTexture(texture2D, width, height, format, data);
            }
        }

        public Dictionary<string, byte[]> GetProperties(Material material)
        {
            var texture = material.mainTexture;
            if (texture == null)
            {
                throw new InvalidOperationException("Material does not have a texture.");
            }
            
            var texture2D = ToTexture2D(texture);
            return GetProperties(texture2D);
        }

        public Dictionary<string, byte[]> GetProperties(Texture2D texture)
        {
            return _bitmapSerializerFactory.RawBitmapSerializer.Serialize(
                texture.width,
                texture.height,
                ConvertFormatToString(texture.format),
                texture.GetRawTextureData());
        }

        public void SetProperties(Dictionary<string, byte[]> properties, Material material, Mesh mesh)
        {
            if(_bitmapSerializerFactory.IsRawBitmap(properties))
            {
                SetRawBitmapProperties(properties, material, mesh);
            }
        }
        
        private void SetRawBitmapProperties(Dictionary<string, byte[]> properties, Material material, Mesh mesh)
        {
            var rawBitmapSerializer = _bitmapSerializerFactory.RawBitmapSerializer;
            var height = rawBitmapSerializer.HeightSerializer.Deserialize(properties);
            var width = rawBitmapSerializer.WidthSerializer.Deserialize(properties);
            var formatName = rawBitmapSerializer.FormatSerializer.Deserialize(properties);
            var data = rawBitmapSerializer.PixelsSerializer.Deserialize(properties);
            
            var format = ConvertFormatFromString(formatName);
            
            var texture = material.mainTexture;
            if (texture == null)
            {
                var texture2D = CreateTexture(width, height, format, data);
                material.mainTexture = texture2D;
            }
            else
            {
                var texture2D = ToTexture2D(texture);
                UpdateTexture(texture2D, width, height, format, data);
            }
            
            UpdateMesh(mesh, width, height);
        }
        
        private static Texture2D CreateTexture(int width, int height, TextureFormat format, byte[] data)
        {
            var texture = new Texture2D(width, height, format, false);
            texture.LoadRawTextureData(data);
            return texture;
        }
        
        private static void UpdateTexture(Texture2D texture, int width, int height, TextureFormat format, byte[] data)
        {
            texture.Reinitialize(width, height, format, false);
            texture.LoadRawTextureData(data);
            texture.Apply();
        }
        
        private static void UpdateMesh(Mesh mesh, int width, int height)
        {
            var maxSize = width > height
                ? new Vector2(1, (float)height / width)
                : new Vector2((float)width / height, 1);
            
            mesh.vertices = new Vector3[]
            {
                new(0, 0, 0), new(0, maxSize.y, 0), new(maxSize.x, maxSize.y, 0), new(maxSize.x, 0, 0)
            };

            mesh.uv = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
                
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Converts a pixel format from TextureFormat to a string.
        /// </summary>
        /// <param name="format">The texture format.</param>
        /// <returns>The pixel fromat in a string.</returns>
        /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
        private string ConvertFormatToString(TextureFormat format)
        {
            if (!_textureFormatToName.TryGetValue(format, out var formatName))
            {
                ThrowUnsupportedTextureFormat(format.ToString());
            }

            return formatName;
        }
        
        /// <summary>
        /// Converts a pixel format from a string to TextureFormat.
        /// </summary>
        /// <param name="format">The pixel format.</param>
        /// <returns>The texture format.</returns>
        /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
        private TextureFormat ConvertFormatFromString(string format)
        {
            if (!_nameToTextureFormat.TryGetValue(format, out var textureFormat))
            {
                ThrowUnsupportedTextureFormat(format);
            }
            
            return textureFormat;
        }

        private void AssertSupportedTextureFormat(TextureFormat format)
        {
            if (!_textureFormatToName.ContainsKey(format))
            {
                ThrowUnsupportedTextureFormat(format.ToString());
            }
        }

        private static void ThrowUnsupportedTextureFormat(string format)
        {
            throw new ArgumentException($"Unsupported texture format: {format}");
        }
        
        private static Texture2D ToTexture2D(Texture texture)
        {
            if (texture is not Texture2D texture2D)
            {
                throw new InvalidOperationException("Texture is not 2D");
            }

            return texture2D;
        }
    }
}