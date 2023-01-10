using System;
using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp light to Unity.
    /// Converter is able to parse ambient lighting, area light, directional light, point light and spot light.
    /// </summary>
    internal static class LightConverter
    {
        /// <summary>
        /// Converts an Assimp light to Unity and assigns it to a game object.
        /// </summary>
        /// <param name="light">The Assimp light.</param>
        /// <param name="gameObjects">Unity game objects.</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void ConvertToUnity(Assimp.Light light, IReadOnlyDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            if (light.LightType == Assimp.LightSourceType.Ambient)
            {
                SetAmbientLight(light);
            }
            else
            {
                CreateSceneLight(light, gameObjects);
            }
        }

        private static void CreateSceneLight(Assimp.Light light, IReadOnlyDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            if (!gameObjects.TryGetValue(light.Name, out var parentGameObject))
            {
                return;
            }

            var gameObject = new UnityEngine.GameObject(light.Name);
            gameObject.transform.SetParent(parentGameObject.transform);

            var unityLight = gameObject.AddComponent<UnityEngine.Light>();
            unityLight.color = new UnityEngine.Color(light.ColorDiffuse.R, light.ColorDiffuse.G, light.ColorDiffuse.B);

            switch (light.LightType)
            {
                case Assimp.LightSourceType.Area:
                    SetAreaLight(unityLight, gameObject, light);
                    break;
                case Assimp.LightSourceType.Directional:
                    SetDirectionalLight(unityLight, gameObject, light);
                    break;
                case Assimp.LightSourceType.Point:
                    SetPointLight(unityLight, gameObject, light);
                    break;
                case Assimp.LightSourceType.Spot:
                    SetSpotLight(unityLight, gameObject, light);
                    break;
                default:
                    throw new NotSupportedException($"Light type \"{light.LightType}\" is not supported.");
            }
        }

        private static void SetAmbientLight(Assimp.Light light)
        {
            /*
             * Ambient lighting in Unity is not placed in a scene.
             * It must be set to global lighting settings.
             */
            UnityEngine.RenderSettings.ambientLight = new UnityEngine.Color(light.ColorAmbient.R, light.ColorAmbient.G, light.ColorAmbient.B);
            UnityEngine.RenderSettings.ambientIntensity = 1;
            UnityEngine.RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        }

        /// <summary>
        /// Sets properties of an area light.
        /// </summary>
        /// <param name="unityLight">Unity light.</param>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetAreaLight(UnityEngine.Light unityLight, UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            unityLight.type = UnityEngine.LightType.Area;
            unityLight.areaSize = new UnityEngine.Vector2(light.AreaSize.X, light.AreaSize.Y);
            SetDirection(lightGameObject, light);
            SetPosition(lightGameObject, light);
        }

        /// <summary>
        /// Sets properties of a directional light.
        /// </summary>
        /// <param name="unityLight">Unity light.</param>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetDirectionalLight(UnityEngine.Light unityLight, UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            unityLight.type = UnityEngine.LightType.Directional;
            SetDirection(lightGameObject, light);
        }

        /// <summary>
        /// Sets properties of a point light.
        /// </summary>
        /// <param name="unityLight">Unity light.</param>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetPointLight(UnityEngine.Light unityLight, UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            unityLight.type = UnityEngine.LightType.Point;
            SetPosition(lightGameObject, light);
        }

        /// <summary>
        /// Sets properties of a spot light.
        /// </summary>
        /// <param name="unityLight">Unity light.</param>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetSpotLight(UnityEngine.Light unityLight, UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            unityLight.type = UnityEngine.LightType.Spot;
            unityLight.shape = UnityEngine.LightShape.Cone;
            unityLight.spotAngle = UnityEngine.Mathf.Rad2Deg * light.AngleInnerCone;
            SetDirection(lightGameObject, light);
            SetPosition(lightGameObject, light);
        }

        /// <summary>
        /// Sets position of a light. Changes transform of a light game object.
        /// </summary>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetPosition(UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            lightGameObject.transform.localPosition = new UnityEngine.Vector3(light.Position.X, light.Position.Y, light.Position.Z);
        }

        /// <summary>
        /// Sets direction of a light. Changes transform of a light game object.
        /// </summary>
        /// <param name="lightGameObject">Light game object.</param>
        /// <param name="light">Assimp light.</param>
        private static void SetDirection(UnityEngine.GameObject lightGameObject, Assimp.Light light)
        {
            // TODO: nastavit rotaci světla
            //lightGameObject.transform.localRotation = light.Direction
        }
    }
}