using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp camera to Unity.
    /// </summary>
    public static class CameraConverter
    {
        /// <summary>
        /// Converts an Assimp camera to Unity and assigns it to a game object.
        /// </summary>
        /// <param name="camera">The Assimp camera.</param>
        /// <param name="gameObjects">Unity game objects.</param>
        public static void ConvertToUnity(Assimp.Camera camera, IReadOnlyDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            if (gameObjects.TryGetValue(camera.Name, out var parentGameObject))
            {
                var gameObject = CreateCameraObject(camera, parentGameObject);
                var cameraComponent = CreateCameraComponent(camera, gameObject);
                // TODO: natočení kamery
            }
        }

        public static UnityEngine.Camera CreateCameraComponent(Assimp.Camera camera, UnityEngine.GameObject gameObject)
        {
            var unityCamera = gameObject.AddComponent<UnityEngine.Camera>();
            unityCamera.nearClipPlane = camera.ClipPlaneNear;
            unityCamera.farClipPlane = camera.ClipPlaneFar;
            
            // When AspectRatio is zero it means it is not specified
            if (camera.AspectRatio != 0)
            {
                unityCamera.aspect = camera.AspectRatio;
            }
            
            unityCamera.fieldOfView = ConvertFieldOfView(camera.FieldOfview, unityCamera.aspect);

            return unityCamera;
        }

        public static UnityEngine.GameObject CreateCameraObject(Assimp.Camera camera, UnityEngine.GameObject parentGameObject)
        {
            return new UnityEngine.GameObject(camera.Name)
            {
                transform =
                {
                    parent = parentGameObject.transform,
                    localPosition = new UnityEngine.Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z)
                }
            };
        }

        /// <param name="fieldOfView">Horizontal field of view in radians</param>
        /// <returns>Vertical field of view in degrees</returns>
        private static float ConvertFieldOfView(float fieldOfView, float aspectRatio)
        {
            return UnityEngine.Camera.HorizontalToVerticalFieldOfView(UnityEngine.Mathf.Rad2Deg * fieldOfView, aspectRatio);
        }
    }
}
