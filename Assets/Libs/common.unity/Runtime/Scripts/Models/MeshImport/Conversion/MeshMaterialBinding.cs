namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Binding between a mesh and a material.
    /// </summary>
    internal class MeshMaterialBinding
    {
        /// <summary>
        /// Gets a material.
        /// </summary>
        public UnityEngine.Material Material { get; }

        /// <summary>
        /// Gets a mesh.
        /// </summary>
        public UnityEngine.Mesh Mesh { get; }

        /// <summary>
        /// Creates a new binding.
        /// </summary>
        /// <param name="mesh">A mesh.</param>
        /// <param name="material">A material.</param>
        public MeshMaterialBinding(UnityEngine.Mesh mesh, UnityEngine.Material material)
        {
            this.Material = material;
            this.Mesh = mesh;
        }
    }
}
