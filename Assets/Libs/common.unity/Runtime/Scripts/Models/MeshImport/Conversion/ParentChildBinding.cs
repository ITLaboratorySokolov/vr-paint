namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Binding between a parent game object and child node.
    /// </summary>
    internal class ParentChildBinding
    {
        /// <summary>
        /// Gets a parent game object.
        /// </summary>
        public UnityEngine.GameObject Parent { get; }

        /// <summary>
        /// Gets a child node.
        /// </summary>
        public Assimp.Node Child { get; }

        /// <summary>
        /// Creates a new binding between a game object and a node.
        /// </summary>
        /// <param name="parent">Parent game object.</param>
        /// <param name="child">Child node.</param>
        public ParentChildBinding(UnityEngine.GameObject parent, Assimp.Node child)
        {
            this.Parent = parent;
            this.Child = child;
        }
    }
}
