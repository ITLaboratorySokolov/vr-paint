using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion
{
    /// <summary>
    /// Convertor of an Assimp scene to Unity.
    /// </summary>
    internal class SceneConverter
    {
        // Different convertors
        private NodeConverter nodeConverter = new NodeConverter();
        private CameraConverter cameraConverter = new CameraConverter();
        private AnimationConverter animationConverter = new AnimationConverter();
        private LightConverter lightConverter = new LightConverter();
        private MaterialConverter materialConverter = new MaterialConverter();
        private TextureConverter textureConverter = new TextureConverter();
        private MeshConverter meshConverter = new MeshConverter();

        /// <summary>
        /// Converts an Assimp scene to Unity game objects.
        /// </summary>
        /// <param name="scene">The Assimp scene.</param>
        public void ConvertToUnity(Assimp.Scene scene)
        {
            List<UnityEngine.Texture> unityTextures = scene.HasTextures ? this.ConvertTexturesToUnity(scene.Textures) : null;
            List<UnityEngine.Material> unityMaterials = scene.HasMaterials ? this.ConvertMaterialsToUnity(scene.Materials, unityTextures) : null;
            List<MeshMaterialBinding> unityMeshes = scene.HasMeshes ? this.ConvertMeshesToUnity(scene.Meshes, unityMaterials) : null;

            Dictionary<string, UnityEngine.GameObject> gameObjects = this.ConvertNodeTreeToUnity(scene.RootNode, unityMeshes);

            if (scene.HasLights) 
            {
                this.ConvertLightsToUnity(scene.Lights, gameObjects);
            }

            if (scene.HasCameras) 
            { 
                this.ConvertCamerasToUnity(scene.Cameras, gameObjects); 
            }

            List<NodeClipBinding> unityAnimations = scene.HasAnimations ? ConvertAnimationsToUnity(scene.Animations) : null;
        }

        /// <summary>
        /// Converts all Assimp meshes to Unity and links materials with them.
        /// </summary>
        /// <param name="meshes">Assimp meshes.</param>
        /// <param name="unityMaterials">Unity materials.</param>
        /// <returns>List of bindings between meshes and materials.</returns>
        public List<MeshMaterialBinding> ConvertMeshesToUnity(List<Assimp.Mesh> meshes, List<UnityEngine.Material> unityMaterials)
        {
            return meshes.ConvertAll(mesh => this.meshConverter.ConvertToUnity(mesh, unityMaterials));
        }

        /// <summary>
        /// Converts all Assimp materials to Unity and assigns texture to them.
        /// </summary>
        /// <param name="materials">Assimp materials.</param>
        /// <param name="unityTextures">Unity textures.</param>
        /// <returns>List of materials.</returns>
        public List<UnityEngine.Material> ConvertMaterialsToUnity(List<Assimp.Material> materials, List<UnityEngine.Texture> unityTextures)
        {
            return materials.ConvertAll(material => this.materialConverter.ConvertToUnity(material, unityTextures));
        }

        /// <summary>
        /// Converts all Assimp textures to Unity
        /// </summary>
        /// <param name="textures">Assimp textures.</param>
        /// <returns>List of Unity textures.</returns>
        public List<UnityEngine.Texture> ConvertTexturesToUnity(List<Assimp.EmbeddedTexture> textures)
        {
            return textures.ConvertAll(texture => this.textureConverter.ConvertToUnity(texture));
        }

        /// <summary>
        /// Converts all Assimp lights to Unity. Lights are assigned to game objects. 
        /// </summary>
        /// <param name="lights">Assimp lights.</param>
        /// <param name="gameObjects">Unity game objects.</param>
        public void ConvertLightsToUnity(List<Assimp.Light> lights, Dictionary<string, UnityEngine.GameObject> gameObjects)
        {
            lights.ForEach(light => this.lightConverter.ConvertToUnity(light, gameObjects));
        }

        /// <summary>
        /// Converts all animations to Unity.
        /// </summary>
        /// <param name="animations">Assimp animations.</param>
        /// <returns></returns>
        public List<NodeClipBinding> ConvertAnimationsToUnity(List<Assimp.Animation> animations)
        {
            return animations.ConvertAll(animation => this.animationConverter.ConvertToUnity(animation));
        }

        /// <summary>
        /// Converts all Assimp cameras to Unity. Cameras are assigned to game objects.
        /// </summary>
        /// <param name="cameras">Assimp cameras.</param>
        /// <param name="gameObjects">Unity game objects.</param>
        public void ConvertCamerasToUnity(List<Assimp.Camera> cameras, Dictionary<string, UnityEngine.GameObject> gameObjects)
        {
            cameras.ForEach(camera => this.cameraConverter.ConvertToUnity(camera, gameObjects));
        }

        /// <summary>
        /// Converts Assimp node tree to Unity game object structure.
        /// </summary>
        /// <param name="rootNode">Root node of the Assimp tree.</param>
        /// <param name="unityMeshes">Unity meshes already converted from Assimp.</param>
        /// <returns>Dictionary of game objects and their names.</returns>
        public Dictionary<string, UnityEngine.GameObject> ConvertNodeTreeToUnity(Assimp.Node rootNode, List<MeshMaterialBinding> unityMeshes)
        {
            var gameObjects = new Dictionary<string, UnityEngine.GameObject>();

            UnityEngine.GameObject rootObject = this.nodeConverter.ConvertToUnity(rootNode, unityMeshes);
            gameObjects.Add(rootObject.name, rootObject);

            // Fill stack with children of the root node
            Stack<ParentChildBinding> stack = new Stack<ParentChildBinding>();
            if (rootNode.HasChildren)
            {
                foreach (Assimp.Node child in rootNode.Children)
                {
                    stack.Push(new ParentChildBinding(rootObject, child));
                }
            }

            // DFS over the node tree
            while(stack.Count > 0)
            {
                ParentChildBinding binding = stack.Pop();
                UnityEngine.GameObject gameObject = this.nodeConverter.ConvertToUnity(binding.Child, unityMeshes);
                gameObject.transform.SetParent(binding.Parent.transform);
                gameObjects.Add(gameObject.name, gameObject);

                // Add new children to the stack
                if(binding.Child.HasChildren)
                {
                    foreach (Assimp.Node child in binding.Child.Children)
                    {
                        stack.Push(new ParentChildBinding(gameObject, child));
                    }
                }
            }

            return gameObjects;
        }
    }
}
