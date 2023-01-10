using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Models.Utility.Events
{
    /// <summary>
    /// Arguments of <see cref="ReportTransformChange.TransformChanged"/> event.
    /// </summary>
    public class TransformChangedEventArgs
    {
        /// <summary>
        /// New transform.
        /// </summary>
        public Transform Transform { get; set; }
    }
}
