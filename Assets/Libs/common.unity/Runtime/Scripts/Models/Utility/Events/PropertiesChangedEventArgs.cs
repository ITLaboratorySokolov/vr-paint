using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.Utility.Events
{
    /// <summary>
    /// Arguments of <see cref="ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.IPropertiesManager.PropertiesChanged"/> event.
    /// </summary>
    public class PropertiesChangedEventArgs
    {
        /// <summary>
        /// Name of the object that triggered the event.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Changed properties.
        /// </summary>
        public Dictionary<string, byte[]> Properties { get; set; }
    }
}
