using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Convertor of an Assimp scene to Unity.
    /// </summary>
    internal static class SceneConverter
    {
        /// <summary>
        /// Converts an Assimp scene to Unity game objects.
        /// </summary>
        /// <param name="scene">The Assimp scene.</param>
        public static void ConvertToUnity(Assimp.Scene scene)
        {
            var unityTextures = scene.HasTextures ? ConvertTexturesToUnity(scene.Textures) : null;
            var unityMaterials = scene.HasMaterials ? ConvertMaterialsToUnity(scene.Materials, unityTextures) : null;
            var unityMeshes = scene.HasMeshes ? ConvertMeshesToUnity(scene.Meshes, unityMaterials) : null;
            
            var gameObjects = ConvertNodeTreeToUnity(scene.RootNode, unityMeshes);

            if (scene.HasLights) 
            {
                ConvertLightsToUnity(scene.Lights, gameObjects);
            }

            if (scene.HasCameras) 
            { 
                ConvertCamerasToUnity(scene.Cameras, gameObjects); 
            }

            var unityAnimations = scene.HasAnimations ? ConvertAnimationsToUnity(scene.Animations) : null;
        }
        
        private static List<MeshMaterialBinding> ConvertMeshesToUnity(List<Assimp.Mesh> meshes, IReadOnlyList<UnityEngine.Material> unityMaterials)
        {
            return meshes.ConvertAll(mesh => MeshConverter.ConvertToUnity(mesh, unityMaterials));
        }
        
        private static List<UnityEngine.Material> ConvertMaterialsToUnity(List<Assimp.Material> materials, IReadOnlyList<UnityEngine.Texture> unityTextures)
        {
            return materials.ConvertAll(material => MaterialConverter.ConvertToUnity(material, unityTextures));
        }
        
        private static List<UnityEngine.Texture> ConvertTexturesToUnity(List<Assimp.EmbeddedTexture> textures)
        {
            return textures.ConvertAll(TextureConverter.ConvertToUnity);
        }

        private static void ConvertLightsToUnity(List<Assimp.Light> lights, IReadOnlyDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            lights.ForEach(light => LightConverter.ConvertToUnity(light, gameObjects));
        }

        private static List<NodeClipBinding> ConvertAnimationsToUnity(List<Assimp.Animation> animations)
        {
            return animations.ConvertAll(AnimationConverter.ConvertToUnity);
        }

        private static void ConvertCamerasToUnity(List<Assimp.Camera> cameras, IReadOnlyDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            cameras.ForEach(camera => CameraConverter.ConvertToUnity(camera, gameObjects));
        }

        private static Dictionary<string, UnityEngine.GameObject> ConvertNodeTreeToUnity(Assimp.Node rootNode, IReadOnlyList<MeshMaterialBinding> unityMeshes)
        {
            var gameObjects = new Dictionary<string, UnityEngine.GameObject>();

            var rootObject = NodeConverter.ConvertToUnity(rootNode, unityMeshes);
            gameObjects.Add(rootObject.name, rootObject);
            
            var stack = InitializeStack(rootNode, rootObject);
            ConvertTreeNodes(unityMeshes, stack, gameObjects);

            return gameObjects;
        }

        // DFS over the node tree
        private static void ConvertTreeNodes(IReadOnlyList<MeshMaterialBinding> unityMeshes, Stack<ParentChildBinding> stack, IDictionary<string, UnityEngine.GameObject> gameObjects)
        {
            while (stack.Count > 0)
            {
                ParentChildBinding binding = stack.Pop();
                UnityEngine.GameObject gameObject = NodeConverter.ConvertToUnity(binding.Child, unityMeshes);
                gameObject.transform.SetParent(binding.Parent.transform);
                gameObjects.Add(gameObject.name, gameObject);

                // Add new children to the stack
                if (binding.Child.HasChildren)
                {
                    foreach (var child in binding.Child.Children)
                    {
                        stack.Push(new ParentChildBinding(gameObject, child));
                    }
                }
            }
        }

        private static Stack<ParentChildBinding> InitializeStack(Assimp.Node rootNode, UnityEngine.GameObject rootObject)
        {
            var stack = new Stack<ParentChildBinding>();
            
            // Fill stack with children of the root node
            if (rootNode.HasChildren)
            {
                foreach (var child in rootNode.Children)
                {
                    stack.Push(new ParentChildBinding(rootObject, child));
                }
            }

            return stack;
        }
    }
}
