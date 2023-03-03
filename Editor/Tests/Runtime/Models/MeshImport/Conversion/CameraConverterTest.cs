using Assimp;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion;
using Camera = Assimp.Camera;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Models.MeshImport.Conversion
{
    [RequiresPlayMode(false)]
    [TestFixture]
    public class CameraConverterTest
    {
        #region CreateCameraObject
        
        [Test]
        public void CreateCameraObject_Name_IsTheSame()
        {
            var parentObject = new GameObject();
            var camera = new Camera { Name = "camera" };

            var gameObject = CameraConverter.CreateCameraObject(camera, parentObject);
            
            Assert.That(gameObject.name, Is.EqualTo(camera.Name));
        }
        
        [Test]
        public void CreateCameraObject_ParentTransform_IsFromParentObject()
        {
            var parentObject = new GameObject();
            var camera = new Camera();

            var gameObject = CameraConverter.CreateCameraObject(camera, parentObject);
            
            Assert.That(gameObject.transform.parent, Is.EqualTo(parentObject.transform));
        }
        
        [Test]
        public void CreateCameraObject_Position_IsTheSame()
        {
            var parentObject = new GameObject();
            var camera = new Camera { Position = new Vector3D(1, 2, 3) };

            var gameObject = CameraConverter.CreateCameraObject(camera, parentObject);

            var localPosition = gameObject.transform.localPosition;
            Assert.That(localPosition.x, Is.EqualTo(1));
            Assert.That(localPosition.y, Is.EqualTo(2));
            Assert.That(localPosition.z, Is.EqualTo(3));
        }
        
        #endregion
        
        #region CreateCameraComponent

        [Test]
        public void CreateCameraComponent_CameraIsPerspective_OrthographicIsFalse()
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera();

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.orthographic, Is.False);
        }
        
        [Test]
        public void CreateCameraComponent_NearClipPane_KeepsTheValue()
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera { ClipPlaneNear = 1 };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.nearClipPlane, Is.EqualTo(1));
        }
        
        [Test]
        public void CreateCameraComponent_FarClipPane_KeepsTheValue()
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera { ClipPlaneFar = 1 };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.farClipPlane, Is.EqualTo(1));
        }

        [Test]
        public void CreateCameraComponent_FarClipPaneLesserThanNearClipPane_BothKeepTheirInputValues()
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera
            {
                ClipPlaneNear = 2, 
                ClipPlaneFar = 1
            };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.nearClipPlane, Is.EqualTo(2));
            Assert.That(unityCamera.farClipPlane, Is.EqualTo(1));
        }

        [TestCase(0.25f * Mathf.PI, 1, 45f)]
        [TestCase(0.25f * Mathf.PI, 2, 23.4018383f)]
        [TestCase(0.25f * Mathf.PI, 0.5f, 79.2785492f)]
        [TestCase(0.5f * Mathf.PI, 1, 90f)]
        [TestCase(0.5f * Mathf.PI, 2, 53.1301041f)]
        [TestCase(0.5f * Mathf.PI, 0.5f, 126.869904f)]
        [TestCase(0.75f * Mathf.PI, 1, 135f)]
        [TestCase(0.75f * Mathf.PI, 2, 100.721451f)]
        [TestCase(0.75f * Mathf.PI, 0.5f, 156.59816f)]
        public void CreateCameraComponent_HorizontalFieldOfViewWithAspect_IsConvertedToVertical(float horizontalFieldOfViewRad, float aspectRatio, float expectedVerticalFieldOfViewDeg)
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera
            {
                FieldOfview = horizontalFieldOfViewRad, 
                AspectRatio = aspectRatio
            };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.fieldOfView, Is.EqualTo(expectedVerticalFieldOfViewDeg));
        }
        
        /*
         * Calculate edge cases:
         * 2 * arctan(tan(x/2)) < 0.00001°
         * arctan(y) > 0.000005°   / y = tan(x/2)
         * y < 8.72665 * 10^-8
         * tan(x/2) < 8.72665 * 10^-8
         * 2*PI*n ≈ x, n ∈ Z
         */
        [TestCase(0f)]
        [TestCase(-0.1f)]
        [TestCase(-0.25f)]
        [TestCase(-0.5f)]
        [TestCase(-1f)]
        [TestCase(-0.5f * Mathf.PI)]
        [TestCase(-0.75f * Mathf.PI)]
        [TestCase(-Mathf.PI + 0.1f)]
        [TestCase(-2 * Mathf.PI)]
        [TestCase(2 * Mathf.PI)]
        public void CreateCameraComponent_HorizontalFieldOfView_IsConvertedToVerticalWithMinValue(float fieldOfView)
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera
            {
                FieldOfview = fieldOfView, 
                AspectRatio = 1
            };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.fieldOfView, Is.EqualTo(0.00001f).Within(0.000001));
        }
        
        /*
         * Calculate edge cases:
         * 2 * arctan(tan(x/2)) > 179°
         * arctan(y) > 89.5°   / y = tan(x/2)
         * x > 114.589
         * tan(x/2) > 114.589
         * 2*PI*n - 3.15905 < x < 2*PI*n - PI, n ∈ Z
         */
        [TestCase(-5 * Mathf.PI - 0.009f)] // n = -2
        [TestCase(-3 * Mathf.PI - 0.009f)] // n = -1
        [TestCase(-Mathf.PI - 0.009f)] // n = 0
        [TestCase(Mathf.PI - 0.009f)] // n = 1
        [TestCase(3 * Mathf.PI - 0.009f)] // n = 2
        public void CreateCameraComponent_HorizontalFieldOfView_IsConvertedToVerticalWithMaxValue(float fieldOfView)
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera { FieldOfview = fieldOfView };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.fieldOfView, Is.EqualTo(179f));
        }
        
        [Test]
        public void CreateCameraComponent_AspectRatioNotSpecified_IsDefaultValue()
        {
            Screen.SetResolution(1920, 1080, false);
            var cameraGameObject = new GameObject();
            var camera = new Camera();

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.aspect, Is.EqualTo(1920f/1080f));
        }
        
        [TestCase(0.0001f)]
        [TestCase(0.1f)]
        [TestCase(0.5f)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void CreateCameraComponent_AspectRatio_KeepsTheValue(float aspectRatio)
        {
            var cameraGameObject = new GameObject();
            var camera = new Camera { AspectRatio = aspectRatio };

            var unityCamera = CameraConverter.CreateCameraComponent(camera, cameraGameObject);
            
            Assert.That(unityCamera.aspect, Is.EqualTo(aspectRatio));
        }
        
        #endregion
    }
}