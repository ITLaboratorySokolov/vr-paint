using System;
using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp mesh to Unity.
    /// </summary>
    internal static class MeshConverter
    {
        /// <summary>
        /// Converts an Assimp mesh to Unity and assigns it materials it is using.
        /// </summary>
        /// <param name="mesh">Assimp mesh.</param>
        /// <param name="unityMaterials">List of all materials in a scene.</param>
        /// <returns>Binding between a mesh and a material.</returns>
        public static MeshMaterialBinding ConvertToUnity(Assimp.Mesh mesh, IReadOnlyList<UnityEngine.Material> unityMaterials)
        {
            var unityMesh = new UnityEngine.Mesh { name = mesh.Name };

            // Set vertices
            if (mesh.HasVertices)
            {
                unityMesh.SetVertices(ConvertVerticesToUnity(mesh.Vertices));
            }

            // Set indices
            if (mesh.HasFaces)
            {
                unityMesh.SetIndices(ConvertFacesToUnity(mesh.Faces), ConvertTopologyToUnity(mesh.PrimitiveType), 0);
            }

            // Set normals
            if (mesh.HasNormals)
            {
                unityMesh.SetNormals(ConvertNormalsToUnity(mesh.Normals));
            }

            // Set vertex colors
            if (mesh.HasVertexColors(0))
            {
                unityMesh.SetColors(ConvertColorsToUnity(mesh.VertexColorChannels[0]));
            }

            // Set tangents
            if (mesh.HasTangentBasis)
            {
                unityMesh.SetTangents(ConvertTangentsToUnity(mesh.Tangents));
            }

            // Set texture coordinates
            for (int i = 0; i < mesh.TextureCoordinateChannelCount; i++)
            {
                if (mesh.HasTextureCoords(i))
                {
                    unityMesh.SetUVs(i, ConvertUvsToUnity(mesh.TextureCoordinateChannels[i]));
                }
            }

            return new MeshMaterialBinding(unityMesh, unityMaterials[mesh.MaterialIndex]);
        }

        /// <summary>
        /// Converts vertices from Assimp vector type to Unity vector type.
        /// </summary>
        /// <param name="vertices">Assimp vertices.</param>
        /// <returns>Unity vertices.</returns>
        private static UnityEngine.Vector3[] ConvertVerticesToUnity(List<Assimp.Vector3D> vertices)
        {
            return vertices.ConvertAll(vertex => new UnityEngine.Vector3(vertex.X, vertex.Y, vertex.Z)).ToArray();
        }

        /// <summary>
        /// Converts Assimp faces to list of indices.
        /// </summary>
        /// <param name="faces">Assimp faces.</param>
        /// <returns>List of indices.</returns>
        /// <exception cref="NotSupportedException">Thrown when a primitive is not supported.</exception>
        private static List<int> ConvertFacesToUnity(List<Assimp.Face> faces)
        {
            var indices = new List<int>();

            foreach (var face in faces)
            {
                if (face.IndexCount <= 4) // Biggest primitive Unity supports is a quad, but Assimp mesh may contain polygons.
                {
                    indices.AddRange(face.Indices);
                } else
                {
                    throw new NotSupportedException("Biggest supported face is a quad, but the provided mesh has bigger faces.");
                }
                
            }

            return indices;
        }

        /// <summary>
        /// Converts Assimp primitive type to Unity mesh topology.
        /// </summary>
        /// <param name="primitiveType">Assimp primitive type.</param>
        /// <returns>Unity mesh topology.</returns>
        /// <exception cref="NotSupportedException">Thrown when the primitive type is not supported.</exception>
        private static UnityEngine.MeshTopology ConvertTopologyToUnity(Assimp.PrimitiveType primitiveType)
        {
            return primitiveType switch
            {
                Assimp.PrimitiveType.Triangle => UnityEngine.MeshTopology.Triangles,
                Assimp.PrimitiveType.Point => UnityEngine.MeshTopology.Points,
                Assimp.PrimitiveType.Line => UnityEngine.MeshTopology.Lines,
                Assimp.PrimitiveType.Polygon => UnityEngine.MeshTopology.Quads,
                _ => throw new NotSupportedException("Primitive type is not supported.")
            };
        }

        /// <summary>
        /// Converts normals from Assimp vector type to Unity vector type.
        /// </summary>
        /// <param name="normals">Assimp normals.</param>
        /// <returns>Unity normals.</returns>
        private static UnityEngine.Vector3[] ConvertNormalsToUnity(List<Assimp.Vector3D> normals)
        {
            return normals.ConvertAll(normal => new UnityEngine.Vector3(normal.X, normal.Y, normal.Z)).ToArray();
        }
        
        /// <summary>
        /// Converts tangents from Assimp vector type to Unity vector type.
        /// </summary>
        /// <param name="tangents">Assimp tangents.</param>
        /// <returns>Unity tangents.</returns>
        private static UnityEngine.Vector4[] ConvertTangentsToUnity(List<Assimp.Vector3D> tangents)
        {
            return tangents.ConvertAll(tangent => new UnityEngine.Vector4(tangent.X, tangent.Y, tangent.Z)).ToArray();
        }

        /// <summary>
        /// Converts texture coordinates from Assimp vector type to Unity vector type.
        /// </summary>
        /// <param name="textureCoordinateChannel">Assimp texture coordinates.</param>
        /// <returns>Unity texture coordinates.</returns>
        private static UnityEngine.Vector2[] ConvertUvsToUnity(List<Assimp.Vector3D> textureCoordinateChannel)
        {
            return textureCoordinateChannel.ConvertAll(textureCoordinate => new UnityEngine.Vector2(textureCoordinate.X, textureCoordinate.Y)).ToArray();
        }

        /// <summary>
        /// Converts colors from Assimp type to Unity type.
        /// </summary>
        /// <param name="colors">Assimp colors.</param>
        /// <returns>Unity colors.</returns>
        private static UnityEngine.Color[] ConvertColorsToUnity(List<Assimp.Color4D> colors)
        {
            return colors.ConvertAll(color => new UnityEngine.Color(color.R, color.G, color.B, color.A)).ToArray();
        }
    }
}