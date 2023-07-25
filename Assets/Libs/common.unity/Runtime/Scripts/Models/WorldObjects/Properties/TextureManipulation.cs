using System;
using System.Collections.Generic;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Bitmap;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties
{
    public class TextureManipulation
    {
        private readonly Dictionary<string, TextureFormat> _nameToTextureFormat = new();
        private readonly Dictionary<TextureFormat, string> _textureFormatToName = new(); 
        private readonly BitmapSerializerFactory _bitmapSerializerFactory = new();

        public TextureManipulation(List<PixelFormat> supportedPixelFormats)
        {
            foreach (var pixelFormat in supportedPixelFormats)
            {
                _nameToTextureFormat.Add(pixelFormat.Name, pixelFormat.Format);
                _textureFormatToName.Add(pixelFormat.Format, pixelFormat.Name);
            }
        }

        /// <returns>Old texture so it can be freed if necessary.</returns>
        public Texture SetTextureWithSupportedFormat(Texture2D newTexture, Material material)
        {
            AssertSupportedTextureFormat(newTexture.format);

            var oldTexture = material.mainTexture;
            material.mainTexture = newTexture;
            return oldTexture;
        }

        public void SetTextureDataToMaterial(int width, int height, TextureFormat format, byte[] data, Material material)
        {
            AssertSupportedTextureFormat(format);
            CreateOrUpdateExistingTexture(width, height, format, data, material);
        }

        public Dictionary<string, byte[]> GetTextureProperties(Material material)
        {
            var texture2D = GetTexture2DFromMaterial(material);
            return GetTextureProperties(texture2D);
        }

        public static Texture2D GetTexture2DFromMaterial(Material material)
        {
            var texture = material.mainTexture;
            if (texture == null)
            {
                throw new InvalidOperationException("Material does not have a texture.");
            }

            var texture2D = ToTexture2D(texture);
            return texture2D;
        }

        public Dictionary<string, byte[]> GetTextureProperties(Texture2D texture)
        {
            return _bitmapSerializerFactory.RawBitmapSerializer.Serialize(
                texture.width,
                texture.height,
                ConvertFormatToString(texture.format),
                texture.GetRawTextureData());
        }

        public void SetTexturePropertiesToMaterial(Dictionary<string, byte[]> properties, Material material)
        {
            if(_bitmapSerializerFactory.IsRawBitmap(properties))
            {
                SetRawBitmapProperties(properties, material);
            }
        }

        private void SetRawBitmapProperties(Dictionary<string, byte[]> properties, Material material)
        {
            var rawBitmapSerializer = _bitmapSerializerFactory.RawBitmapSerializer;
            var height = rawBitmapSerializer.HeightSerializer.Deserialize(properties);
            var width = rawBitmapSerializer.WidthSerializer.Deserialize(properties);
            var formatName = rawBitmapSerializer.FormatSerializer.Deserialize(properties);
            var data = rawBitmapSerializer.PixelsSerializer.Deserialize(properties);
            
            var format = ConvertFormatFromString(formatName);
            
            CreateOrUpdateExistingTexture(width, height, format, data, material);
        }

        public static void CreateOrUpdateExistingTexture(int width, int height, TextureFormat format, byte[] data,
            Material material)
        {
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

        private static Texture2D CreateTexture(int width, int height, TextureFormat format, byte[] data)
        {
            var texture = new Texture2D(width, height, format, false);
            texture.LoadRawTextureData(data);
            texture.Apply();
            return texture;
        }
        
        private static void UpdateTexture(Texture2D texture, int width, int height, TextureFormat format, byte[] data)
        {
            texture.Reinitialize(width, height, format, false);
            texture.LoadRawTextureData(data);
            texture.Apply();
        }

        /// <summary>
        /// Converts a pixel format from TextureFormat to a string.
        /// </summary>
        /// <param name="format">The texture format.</param>
        /// <returns>The pixel fromat in a string.</returns>
        /// <exception cref="ArgumentException">Thrown when unsupported pixel format is passed.</exception>
        public string ConvertFormatToString(TextureFormat format)
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
        public TextureFormat ConvertFormatFromString(string format)
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