using System;
using NUnit.Framework;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;
using Object = UnityEngine.Object;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Behaviours.WorldObjects.Properties.Managers
{
    [TestFixture]
    public class BitmapPropertiesManagerTest
    {
        private GameObject _gameObject;
        private BitmapPropertiesManager _bitmapPropertiesManager;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        [SetUp]
        public void SetUp()
        {
            _gameObject = Object.Instantiate(Resources.Load("BitmapPropertiesManagerTest")) as GameObject;
            _bitmapPropertiesManager = _gameObject.GetComponent<BitmapPropertiesManager>();
            _meshFilter = _gameObject.GetComponent<MeshFilter>();
            _meshRenderer = _gameObject.GetComponent<MeshRenderer>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_gameObject);
        }

        #region SetTexture

        [Test]
        public void SetTexture_FirstCall_OldTextureIsNull()
        {
            var texture = new Texture2D(1, 1);
            
            var oldTexture = _bitmapPropertiesManager.SetTexture(texture);
            
            Assert.That(oldTexture, Is.Null);
        }

        [Test]
        public void SetTexture_MeshRenderer_ContainsTexture()
        {
            var texture = new Texture2D(1, 1);
            
            _ = _bitmapPropertiesManager.SetTexture(texture);

            Assert.That(_meshRenderer.material.mainTexture, Is.EqualTo(texture));
        }
        
        #endregion
        
        #region SetTextureData

        [Test]
        public void SetTextureData_FirstCall_()
        {
            _bitmapPropertiesManager.SetTextureData(1, 1, TextureFormat.RGB24, new byte[3]);
        }
        
        // TODO Dopsat testy
        
        #endregion
        
        // [UnityTest]
        // public IEnumerator SetProperties_CallTenThousandTimes_ClearResources()
        // {
        //     const int times = 10000;
        //     const int width = 1024;
        //     const int height = 1024;
        //     const int pixelsCount = width * height;
        //     const int bytesCount = pixelsCount * 3;
        //     
        //     var pixels = new byte[bytesCount];
        //     for (var i = 0; i < pixels.Length; i++)
        //     {
        //         pixels[i] = 255;
        //     }
        //     
        //     var properties = new Dictionary<string, byte[]>()
        //     {
        //         { "Width", BitConverter.GetBytes(width) },
        //         { "Height", BitConverter.GetBytes(height) },
        //         { "Format", Encoding.UTF8.GetBytes("RGB") },
        //         { "Pixels", pixels }
        //     };
        //
        //     for (var i = 0; i < times; i++)
        //     {
        //         this.bitmapPropertiesManager.SetProperties(properties);
        //         yield return null;
        //     }
        // }
        //
        // [UnityTest]
        // public IEnumerator SetTextureData_CallTenThousandTimes_ClearResources()
        // {
        //     const int times = 10000;
        //     const int width = 1024;
        //     const int height = 1024;
        //     const int pixelsCount = width * height;
        //     const int bytesCount = pixelsCount * 3;
        //     
        //     var pixels = new byte[bytesCount];
        //     for (var i = 0; i < pixels.Length; i++)
        //     {
        //         pixels[i] = 255;
        //     }
        //
        //     for (var i = 0; i < times; i++)
        //     {
        //         this.bitmapPropertiesManager.SetTextureData(width, height, TextureFormat.RGB24, pixels);
        //         yield return null;
        //     }
        // }
        //
        // [UnityTest]
        // public IEnumerator SetTexture_CallTenThousandTimes_ClearResources()
        // {
        //     const int times = 10000;
        //     const int width = 1024;
        //     const int height = 1024;
        //     const int pixelsCount = width * height;
        //     const int bytesCount = pixelsCount * 3;
        //     
        //     var pixels = new byte[bytesCount];
        //     for (var i = 0; i < pixels.Length; i++)
        //     {
        //         pixels[i] = 255;
        //     }
        //
        //     for (var i = 0; i < times; i++)
        //     {
        //         var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        //         texture.SetPixelData(pixels, 0);
        //         var oldTexture = this.bitmapPropertiesManager.SetTexture(texture);
        //         Object.Destroy(oldTexture);
        //         yield return null;
        //     }
        // }
    }
}