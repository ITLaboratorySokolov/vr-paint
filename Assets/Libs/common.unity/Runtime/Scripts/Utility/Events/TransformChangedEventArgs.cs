using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Utility.Events
{
    /// <summary>
    /// Arguments of <see cref="ZCU.TechnologyLab.Common.Unity.WorldObjects.ReportTransformChange.TransformChanged"/> event.
    /// </summary>
    public class TransformChangedEventArgs
    {
        /// <summary>
        /// New transform.
        /// </summary>
        public Transform Transform { get; set; }
    }
}
