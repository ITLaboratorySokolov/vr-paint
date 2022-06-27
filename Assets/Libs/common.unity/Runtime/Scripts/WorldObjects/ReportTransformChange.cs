using System;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects
{
    /// <summary>
    /// When placed on a game object, it allows the object to report its changes of a transform.
    /// 
    /// Because transform is separated from other components of a game object, it makes sense to me
    /// to separate its reporting from a properties manager. This way properties manager handles only 
    /// serialization and deserialization to/from dictionary of properties and reports only their changes.
    /// 
    /// Transform does not have to be updated in this class because it will probably be moved by a seraprate
    /// component dependent on an input control. It checks updates in Update method.
    /// 
    /// The <see cref="WorldObjectManager"/> looks for this class and when it founds it on a game object
    /// it catches the <see cref="TransformChanged"/> event and sends updates to a sever.
    /// You can as well update an object manually via <see cref="WorldObjectManager.UpdateObjectAsync"/>.
    /// </summary>
    public class ReportTransformChange : MonoBehaviour
    {
        /// <summary>
        /// Event called when transform is changed.
        /// </summary>
        public event EventHandler<TransformChangedEventArgs> TransformChanged;

        /// <summary>
        /// Should change be reported?
        /// </summary>
        [SerializeField]
        private bool reportTransformChangedEvent = true;

        /// <summary>
        /// Time interval between reports [in miliseconds].
        /// </summary>
        [SerializeField]
        private float transformChangedEventReportDelay = 0.1f;

        /// <summary>
        /// Remaining time to next report. 
        /// </summary>
        private float remainingTime;

        /// <summary>
        /// Gets or sets if a transform changes should be reported.
        /// </summary>
        public bool ReportTransformChangedEvent
        {
            get => this.reportTransformChangedEvent;
            set => this.reportTransformChangedEvent = value;
        }

        /// <summary>
        /// Gets or sets duration between reports.
        /// </summary>
        public float TransformChangedEventReportDelay
        {
            get => this.transformChangedEventReportDelay;
            set => this.transformChangedEventReportDelay = value;
        }

        /// <summary>
        /// Invokes <see cref="TransformChanged"/> event.
        /// </summary>
        private void Update()
        {
            if (this.remainingTime > 0)
            {
                this.remainingTime -= Time.deltaTime;
            }

            if (this.transform.hasChanged && this.reportTransformChangedEvent && this.remainingTime <= 0)
            {
                this.remainingTime = this.transformChangedEventReportDelay;
                this.transform.hasChanged = false;
                this.TransformChanged?.Invoke(this, new TransformChangedEventArgs { Transform = this.transform });
            }

        }
    }
}
