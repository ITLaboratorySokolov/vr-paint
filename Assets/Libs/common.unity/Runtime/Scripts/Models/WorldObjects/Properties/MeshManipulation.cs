using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Models.Utility;

namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties
{
    public class MeshManipulation
    {
        /// <summary>
        /// Supported mesh primitives.
        /// </summary>
        private static readonly string[] SupportedPrimitives = { "Triangle" };
        
        /// <summary>
        /// Mesh serializer factory.
        /// </summary>
        private readonly MeshSerializerFactory _meshSerializerFactory = new();

        private readonly TextureManipulation _textureManipulation;

        public MeshManipulation(List<PixelFormat> supportedPixelFormats)
        {
            _textureManipulation = new TextureManipulation(supportedPixelFormats);
        }
        
        public Dictionary<string, byte[]> GetPropertiesFromMesh(Mesh mesh, Material material)
        {
            if (material.mainTexture == null)
            {
                return _meshSerializerFactory.RawMeshSerializer.Serialize(
                    VectorConverter.Vector3ToFloat(mesh.vertices),
                    mesh.triangles,
                    SupportedPrimitives[0]);
            }

            var texture = TextureManipulation.GetTexture2DFromMaterial(material);
            return _meshSerializerFactory.RawMeshSerializer.Serialize(
                VectorConverter.Vector3ToFloat(mesh.vertices), 
                mesh.triangles, 
                SupportedPrimitives[0], 
                VectorConverter.Vector2ToFloat(mesh.uv),
                texture.width,
                texture.height,
                _textureManipulation.ConvertFormatToString(texture.format),
                texture.GetRawTextureData());
        }

        public void SetPropertiesToMesh(Dictionary<string, byte[]> properties, Mesh mesh, Material material)
        {
            if(_meshSerializerFactory.IsRawMesh(properties))
            {
                SetRawMeshProperties(properties, mesh, material);
            } else if (_meshSerializerFactory.IsPlyFile(properties))
            {
                // TODO ply file
            }
        }

        private void SetRawMeshProperties(Dictionary<string, byte[]> properties, Mesh mesh, Material material)
        {
            var meshSerializer = _meshSerializerFactory.RawMeshSerializer;
            if (!SupportedPrimitives.Contains(meshSerializer.PrimitiveSerializer.Deserialize(properties)))
            {
                return;
            }

            var vertices = VectorConverter.FloatToVector3(meshSerializer.VerticesSerializer.Deserialize(properties));
            var triangles = meshSerializer.IndicesSerializer.Deserialize(properties);

            if (meshSerializer.UvSerializer.ContainsProperty(properties))
            {
                var uvs = VectorConverter.FloatToVector2(meshSerializer.UvSerializer.Deserialize(properties));
                UpdateMeshVerticesTrianglesAndUvs(vertices, triangles, uvs, mesh);
            }
            else
            {
                UpdateMeshVerticesAndTriangles(vertices, triangles, mesh);
            }

            if (meshSerializer.DiffuseTextureWidthSerializer.ContainsProperty(properties) &&
                meshSerializer.DiffuseTextureHeightSerializer.ContainsProperty(properties) &&
                meshSerializer.DiffuseTextureFormatSerializer.ContainsProperty(properties) &&
                meshSerializer.DiffuseTexturePixelsSerializer.ContainsProperty(properties))
            {
                var width = meshSerializer.DiffuseTextureWidthSerializer.Deserialize(properties);
                var height = meshSerializer.DiffuseTextureHeightSerializer.Deserialize(properties);
                var formatName = meshSerializer.DiffuseTextureFormatSerializer.Deserialize(properties);
                var data = meshSerializer.DiffuseTexturePixelsSerializer.Deserialize(properties);
                
                var format = _textureManipulation.ConvertFormatFromString(formatName);
                TextureManipulation.CreateOrUpdateExistingTexture(width, height, format, data, material);
            }
        }

        public static void UpdateMeshToSize(Mesh mesh, int width, int height)
        {
            var maxSize = width > height
                ? new Vector2(1, (float)height / width)
                : new Vector2((float)width / height, 1);
            
            mesh.vertices = new Vector3[]
            {
                new(0, 0, 0), new(0, maxSize.y, 0), new(maxSize.x, maxSize.y, 0), new(maxSize.x, 0, 0)
            };

            mesh.uv = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
                
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        public static void UpdateMeshVerticesAndTriangles(Vector3[] vertices, int[] triangles, Mesh mesh)
        {
            SetVertices(vertices, mesh);
            mesh.triangles = triangles;
            RecalculateMesh(mesh);
        }
        
        public static void UpdateMeshVerticesTrianglesAndUvs(Vector3[] vertices, int[] triangles, Vector2[] uvs, Mesh mesh)
        {
            UpdateMeshVerticesAndTriangles(vertices, triangles, mesh);
            mesh.uv = uvs;
        }

        public static void RecalculateMesh(Mesh mesh)
        {
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        public static void SetVertices(Vector3[] vertices, Mesh mesh)
        {
            if (vertices.Length > ushort.MaxValue)
            {
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
            
            mesh.vertices = vertices;
        }
    }
}