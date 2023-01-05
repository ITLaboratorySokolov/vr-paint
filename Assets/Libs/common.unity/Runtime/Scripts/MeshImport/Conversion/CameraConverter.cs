using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp camera to Unity.
    /// </summary>
    internal class CameraConverter
    {
        /// <summary>
        /// Converts an Assimp camera to Unity and assigns it to a game object.
        /// </summary>
        /// <param name="camera">The Assimp camera.</param>
        /// <param name="gameObjects">Unity game objects.</param>
        public void ConvertToUnity(Assimp.Camera camera, Dictionary<string, UnityEngine.GameObject> gameObjects)
        {
            if(gameObjects.TryGetValue(camera.Name, out UnityEngine.GameObject parentGameObject))
            {
                var gameObject = new UnityEngine.GameObject(camera.Name);
                gameObject.transform.parent = parentGameObject.transform;
                gameObject.transform.localPosition = new UnityEngine.Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z);
                
                // TODO: natočení kamery

                var unityCamera = gameObject.AddComponent<UnityEngine.Camera>();
                unityCamera.nearClipPlane = camera.ClipPlaneNear;
                unityCamera.farClipPlane = camera.ClipPlaneFar;
                unityCamera.fieldOfView = UnityEngine.Mathf.Rad2Deg * camera.FieldOfview;

                if (camera.AspectRatio != 0)
                {
                    unityCamera.aspect = camera.AspectRatio;
                }
            }
        }
    }
}
