namespace ZCU.TechnologyLab.Common.Unity.Utility.Events
{
    /// <summary>
    /// Arguments of <see cref="ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.IPropertiesManager.PropertyChanged"/> event.
    /// </summary>
    public class PropertyChangedEventArgs
    {
        /// <summary>
        /// Name of the object that triggered the event.
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Name of changed property.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Value of changed property.
        /// </summary>
        public string PropertyValue { get; set; }
    }
}
