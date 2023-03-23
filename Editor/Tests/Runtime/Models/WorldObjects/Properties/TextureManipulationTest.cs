using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Models.WorldObjects.Properties
{
    [RequiresPlayMode(false)]
    [TestFixture]
    public class TextureManipulationTest
    {
        private TextureManipulation _textureManipulation;

        [SetUp]
        public void SetUp()
        {
            _textureManipulation = new TextureManipulation(new List<PixelFormat>
            {
                new() { Name = "RGB", Format = TextureFormat.RGB24 }
            });
        }

        #region Initialization
        
        [Test]
        public void BitmapProperties_RepeatedFormatNames_ThrowsArgumentException()
        {
            var pixelFormats = new List<PixelFormat>
            {
                new() { Name = "name", Format = TextureFormat.Alpha8 },
                new() { Name = "name", Format = TextureFormat.R8 }
            };
            
            Assert.That(() => new TextureManipulation(pixelFormats), Throws.ArgumentException);
        }
        
        [Test]
        public void BitmapProperties_RepeatedFormats_ThrowsArgumentException()
        {
            var pixelFormats = new List<PixelFormat>
            {
                new() { Name = "name", Format = TextureFormat.Alpha8 },
                new() { Name = "name2", Format = TextureFormat.Alpha8 }
            };
            
            Assert.That(() => new TextureManipulation(pixelFormats), Throws.ArgumentException);
        }
        
        #endregion
        
        #region SetTexture

        [Test]
        public void SetTexture_UnsupportedTextureFormat_ThrowsArgumentException()
        {
            var texture = new Texture2D(1, 1, TextureFormat.Alpha8, false);

            Assert.That(() => _textureManipulation.SetTextureWithSupportedFormat(texture, null), Throws.ArgumentException);
        }

        [Test]
        public void SetTexture_SupportedTextureFormat_Passes()
        {
            var texture = new Texture2D(0, 0, TextureFormat.RGB24, false);
            var material = new Material(Shader.Find("Diffuse"));

            _ = _textureManipulation.SetTextureWithSupportedFormat(texture, material);
            
            Assert.Pass();
        }
        
        [Test]
        public void SetTexture_MaterialWithTexture_ReturnsOldTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            var oldTexture = new Texture2D(1, 1);
            var material = new Material(Shader.Find("Diffuse")) { mainTexture = oldTexture };

            var result = _textureManipulation.SetTextureWithSupportedFormat(texture, material);
        
            Assert.That(result, Is.EqualTo(oldTexture));
        }
        
        [Test]
        public void SetTexture_MaterialWithoutTexture_ReturnsNull()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            var material = new Material(Shader.Find("Diffuse"));

            var result = _textureManipulation.SetTextureWithSupportedFormat(texture, material);
        
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void SetTexture_Material_ChangesTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            var material = new Material(Shader.Find("Diffuse"));

            _ = _textureManipulation.SetTextureWithSupportedFormat(texture, material);
            
            Assert.That(material.mainTexture, Is.EqualTo(texture));
        }

        #endregion
        
        #region SetTextureData

        [Test]
        public void SetTextureData_UnsupportedTextureFormat_ThrowsArgumentException()
        {
            const TextureFormat format = TextureFormat.Alpha8;

            Assert.That(() => _textureManipulation.SetTextureDataToMaterial(0, 0 , format, null, null), Throws.ArgumentException);
        }
        
        [Test]
        public void SetTextureData_SupportedTextureFormat_Passes()
        {
            var material = new Material(Shader.Find("Diffuse"));

            _textureManipulation.SetTextureDataToMaterial(0, 0, TextureFormat.RGB24, Array.Empty<byte>(), material);

            Assert.Pass();
        }

        [Test]
        public void SetTextureData_TextureIsNull_IsCreated()
        {
            var material = new Material(Shader.Find("Diffuse"));

            _textureManipulation.SetTextureDataToMaterial(1, 1, TextureFormat.RGB24, new byte[3], material);

            Assert.That(material.mainTexture, Is.Not.Null);
        }
        
        [Test]
        public void SetTextureData_CreatedTexture_WidthIsSet()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];
            
            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            Assert.That(material.mainTexture.width, Is.EqualTo(width));
        }
        
        [Test]
        public void SetTextureData_CreatedTexture_HeightIsSet()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];
            
            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            Assert.That(material.mainTexture.height, Is.EqualTo(height));
        }
        
        [Test]
        public void SetTextureData_CreatedTexture_FormatIsSet()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const TextureFormat format = TextureFormat.RGB24;

            _textureManipulation.SetTextureDataToMaterial(1, 1, format, new byte[3], material);

            var texture = (Texture2D)material.mainTexture;
            Assert.That(texture.format, Is.EqualTo(format));
        }

        [Test]
        public void SetTextureData_CreatedTexture_DataAreSet()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            var texture = (Texture2D)material.mainTexture;
            Assert.That(texture.GetRawTextureData(), Is.EqualTo(data));
        }

        [Test]
        public void SetTextureData_CreatedTextureDataShorterThanNeeded_ThrowsException()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const int width = 1;
            const int height = 2;
            var data = new byte[1];

            Assert.That(() => _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material), Throws.InstanceOf<UnityException>());
        }

        [Test]
        public void SetTextureData_CreatedTextureDataLongerThanNeeded_IsCropped()
        {
            var material = new Material(Shader.Find("Diffuse"));

            const int width = 1;
            const int height = 2;
            var data = new byte[100];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            var texture = (Texture2D)material.mainTexture;
            Assert.That(texture.GetRawTextureData(), Is.EqualTo(new byte[6]));
        }

        [Test]
        public void SetTextureData_TextureIsNot2D_ThrowsException()
        {
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = new RenderTexture(1, 1, 1)
            };

            Assert.That(() => _textureManipulation.SetTextureDataToMaterial(0, 0, TextureFormat.RGB24, Array.Empty<byte>(), material), Throws.InvalidOperationException);
        }
        
        [Test]
        public void SetTextureData_ExistingTexture2D_WidthIsUpdated()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            Assert.That(texture.width, Is.EqualTo(width));
        }
        
        [Test]
        public void SetTextureData_ExistingTexture2D_HeightIsUpdated()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            Assert.That(texture.height, Is.EqualTo(height));
        }
        
        [Test]
        public void SetTextureData_ExistingTexture2D_FormatIsUpdated()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const TextureFormat format = TextureFormat.RGB24;

            _textureManipulation.SetTextureDataToMaterial(1, 1, format, new byte[3], material);

            Assert.That(texture.format, Is.EqualTo(format));
        }
        
        [Test]
        public void SetTextureData_ExistingTexture2D_DataAreUpdated()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const int width = 1;
            const int height = 2;
            var data = new byte[width * height * 3];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);

            Assert.That(texture.GetRawTextureData(), Is.EqualTo(data));
        }

        [Test]
        public void SetTextureData_ExistingTexture2DDataShorterThanNeeded_ThrowsException()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const int width = 1;
            const int height = 2;
            var data = new byte[1];

            Assert.That(() => _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material), Throws.InstanceOf<UnityException>());
        }

        [Test]
        public void SetTextureData_ExistingTexture2DDataLongerThanNeeded_IsCropped()
        {
            var texture = new Texture2D(0, 0);
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = texture
            };

            const int width = 1;
            const int height = 2;
            var data = new byte[100];

            _textureManipulation.SetTextureDataToMaterial(width, height, TextureFormat.RGB24, data, material);
            
            Assert.That(texture.GetRawTextureData(), Is.EqualTo(new byte[6]));
        }

        #endregion
        
        #region GetProperties

        [Test]
        public void GetProperties_UnsupportedTextureFormat_ThrowsArgumentException()
        {
            var texture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
            
            Assert.That(() => _textureManipulation.GetTextureProperties(texture), Throws.ArgumentException);
        }

        [Test]
        public void GetProperties_SupportedTextureFormat_DictionaryValuesAreEqualToInputTexture()
        {
            var texture = new Texture2D(1, 2, TextureFormat.RGB24, false);

            var properties = _textureManipulation.GetTextureProperties(texture);

            Assert.That(string.Join(',', properties["Width"]), Is.EqualTo("1,0,0,0"));
            Assert.That(string.Join(',', properties["Height"]), Is.EqualTo("2,0,0,0"));
            Assert.That(string.Join(',', properties["Format"]), Is.EqualTo("82,71,66"));
            Assert.That(string.Join(',', properties["Pixels"]), Is.EqualTo("205,205,205,205,205,205"));
        }

        [Test]
        public void GetProperties_MaterialWithoutTexture_ThrowsInvalidOperationException()
        {
            var material = new Material(Shader.Find("Diffuse"));

            Assert.That(() => _textureManipulation.GetTextureProperties(material), Throws.InvalidOperationException);
        }
        
        [Test]
        public void GetProperties_MaterialWithTexture2D_DictionaryValuesAreEqualToInputTexture()
        {
            var texture = new Texture2D(1, 2, TextureFormat.RGB24, false);
            var material = new Material(Shader.Find("Diffuse")) { mainTexture = texture };

            var properties = _textureManipulation.GetTextureProperties(material);

            Assert.That(string.Join(',', properties["Width"]), Is.EqualTo("1,0,0,0"));
            Assert.That(string.Join(',', properties["Height"]), Is.EqualTo("2,0,0,0"));
            Assert.That(string.Join(',', properties["Format"]), Is.EqualTo("82,71,66"));
            Assert.That(string.Join(',', properties["Pixels"]), Is.EqualTo("205,205,205,205,205,205"));
        }
        
        [Test]
        public void GetProperties_MaterialWithoutTexture2D_ThrowsInvalidOperationException()
        {
            var texture = new RenderTexture(1, 2, 3);
            var material = new Material(Shader.Find("Diffuse")) { mainTexture = texture };

            Assert.That(() => _textureManipulation.GetTextureProperties(material), Throws.InvalidOperationException);
        }
        
        #endregion

        #region SetProperties

        [Test]
        public void SetProperties_UnsupportedTextureFormat_ThrowsArgumentException()
        {
            var properties = new Dictionary<string, byte[]>
            {
                { "Height", new byte[] { 0, 0, 0, 0 } },
                { "Width", new byte[] { 0, 0, 0, 0 }},
                { "Format", new byte[] { 65 } }, // 65 je A v UTF8
                { "Pixels", Array.Empty<byte>() },
            };
            
            Assert.That(() => _textureManipulation.SetTexturePropertiesToMaterial(properties, null), Throws.ArgumentException);
        }
        
        [Test]
        public void SetProperties_SupportedTextureFormat_Passes()
        {
            var material = new Material(Shader.Find("Diffuse"));
            var mesh = new Mesh();
            var properties = new Dictionary<string, byte[]>
            {
                { "Height", new byte[] { 0, 0, 0, 0 } },
                { "Width", new byte[] { 0, 0, 0, 0 }},
                { "Format", new byte[] { 82, 71, 66 } },
                { "Pixels", Array.Empty<byte>() },
            };

            _textureManipulation.SetTexturePropertiesToMaterial(properties, material);
            
            Assert.Pass();
        }
        
        [Test]
        public void SetProperties_Texture_IsUpdated()
        {
            var data = new byte[2 * 3 * 3];
            var properties = new Dictionary<string, byte[]>
            {
                { "Height", new byte[] { 2, 0, 0, 0 } },
                { "Width", new byte[] { 3, 0, 0, 0 }},
                { "Format", new byte[] { 82, 71, 66 } },
                { "Pixels", data },
            };

            var mesh = new Mesh();
            var material = new Material(Shader.Find("Diffuse"))
            {
                mainTexture = new Texture2D(1, 1),
            };
            
            _textureManipulation.SetTexturePropertiesToMaterial(properties, material);

            var texture = (Texture2D)material.mainTexture;
            Assert.That(texture.height, Is.EqualTo(2));
            Assert.That(texture.width, Is.EqualTo(3));
            Assert.That(texture.format, Is.EqualTo(TextureFormat.RGB24));
            Assert.That(texture.GetRawTextureData(), Is.EqualTo(data));
        }

        #endregion
    }
}