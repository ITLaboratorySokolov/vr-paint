using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp camera to Unity.
    /// </summary>
    internal static class CameraConverter
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

        private static UnityEngine.Camera CreateCameraComponent(Assimp.Camera camera, UnityEngine.GameObject gameObject)
        {
            var unityCamera = gameObject.AddComponent<UnityEngine.Camera>();
            unityCamera.nearClipPlane = camera.ClipPlaneNear;
            unityCamera.farClipPlane = camera.ClipPlaneFar;
            unityCamera.fieldOfView = UnityEngine.Mathf.Rad2Deg * camera.FieldOfview;

            if (camera.AspectRatio != 0)
            {
                unityCamera.aspect = camera.AspectRatio;
            }

            return unityCamera;
        }

        private static UnityEngine.GameObject CreateCameraObject(Assimp.Camera camera, UnityEngine.GameObject parentGameObject)
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
    }
}
