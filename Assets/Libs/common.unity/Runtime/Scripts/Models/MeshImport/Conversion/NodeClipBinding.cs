using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Binding between a node and an animation clip.
    /// </summary>
    public class NodeClipBinding
    {
        public Dictionary<string, List<UnityEngine.AnimationClip>> ClipBindings = new();
    }
}
