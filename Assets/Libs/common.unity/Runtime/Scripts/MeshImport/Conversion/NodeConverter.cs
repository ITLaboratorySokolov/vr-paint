using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp node to Unity game object.
    /// </summary>
    internal class NodeConverter
    {
        /// <summary>
        /// Converts Assimp node to a game object and assigns meshes to game objects.
        /// </summary>
        /// <param name="node">Assimp node.</param>
        /// <param name="unityMeshes">Unity meshes.</param>
        /// <returns>A game object.</returns>
        public UnityEngine.GameObject ConvertToUnity(Assimp.Node node, List<MeshMaterialBinding> unityMeshes)
        {
            var gameObject = new UnityEngine.GameObject(node.Name);

            if (node.HasMeshes)
            {
                foreach (var index in node.MeshIndices)
                {
                    var meshBinding = unityMeshes[index];
                    
                    var meshObject = new UnityEngine.GameObject(meshBinding.Mesh.name);
                    var meshFilter = meshObject.AddComponent<UnityEngine.MeshFilter>();
                    var meshRenderer = meshObject.AddComponent<UnityEngine.MeshRenderer>();
                    var meshCollider = meshObject.AddComponent<UnityEngine.MeshCollider>();

                    meshFilter.mesh = meshBinding.Mesh;
                    meshRenderer.material = meshBinding.Material;
                    meshObject.transform.parent = gameObject.transform;
                }
            }

            node.Transform.Decompose(out Assimp.Vector3D scaling, out Assimp.Quaternion rotation, out Assimp.Vector3D translation);

            gameObject.transform.localPosition = new UnityEngine.Vector3(translation.X, translation.Y, translation.Z);
            gameObject.transform.localRotation = new UnityEngine.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            gameObject.transform.localScale = new UnityEngine.Vector3(scaling.X, scaling.Y, scaling.Z);

            return gameObject;
        }
    }
}
