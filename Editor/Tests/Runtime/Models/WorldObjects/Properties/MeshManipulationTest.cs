using NUnit.Framework;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Properties;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Models.WorldObjects.Properties
{
    public class MeshManipulationTest
    {
        [TestCase(1, 1, "(0.00, 0.00, 0.00),(0.00, 1.00, 0.00),(1.00, 1.00, 0.00),(1.00, 0.00, 0.00)")]
        [TestCase(100, 50, "(0.00, 0.00, 0.00),(0.00, 0.50, 0.00),(1.00, 0.50, 0.00),(1.00, 0.00, 0.00)")]
        [TestCase(50, 100, "(0.00, 0.00, 0.00),(0.00, 1.00, 0.00),(0.50, 1.00, 0.00),(0.50, 0.00, 0.00)")]
        public void UpdateMeshToSize_MeshVertices_ChangeWithSize(int width, int height, string vertices)
        {
            var mesh = new Mesh();
            MeshManipulation.UpdateMeshToSize(mesh, width, height);
            
            Assert.That(string.Join(",", mesh.vertices), Is.EqualTo(vertices));
        }

        [TestCase(1, 1)]
        [TestCase(100, 50)]
        [TestCase(50, 100)]
        public void UpdateMeshToSize_MeshUvs_AreUpdatedAndIndependentOnSize(int width, int height)
        {
            var mesh = new Mesh();
        
            MeshManipulation.UpdateMeshToSize(mesh, width, height);
            
            Assert.That(string.Join(",", mesh.uv), Is.EqualTo("(0.00, 0.00),(0.00, 1.00),(1.00, 1.00),(1.00, 0.00)"));
        }
        
        [TestCase(1, 1)]
        [TestCase(100, 50)]
        [TestCase(50, 100)]
        public void UpdateMeshToSize_MeshTriangles_AreUpdatedAndIndependentOnTextureSize(int width, int height)
        {
            var mesh = new Mesh();
        
            MeshManipulation.UpdateMeshToSize(mesh, width, height);
            
            Assert.That(string.Join(",", mesh.triangles), Is.EqualTo("0,1,2,0,2,3"));
        }
    }
}