using System;
using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Models.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects
{
    /// <summary>
    /// When placed on a game object, it allows the object to report its changes of a transform.
    /// 
    /// Because transform is separated from other components of a game object, it makes sense to me
    /// to separate its reporting from a properties manager. This way properties manager handles only 
    /// serialization and deserialization to/from dictionary of properties and reports only their changes.
    /// 
    /// Transform does not have to be updated in this class because it will probably be moved by a separate
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
        public event Action<TransformChangedEventArgs> TransformChanged;

        /// <summary>
        /// Should change be reported?
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("enableReporting")]
        private bool _enableReporting = true;

        /// <summary>
        /// Time interval between reports [in miliseconds].
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("reportDelay")]
        private float _reportDelay = 0.1f;

        /// <summary>
        /// Gets or sets if a transform changes should be reported.
        /// </summary>
        public bool EnableReporting
        {
            get => _transformChangeReporter.EnableReporting;
            set => _transformChangeReporter.EnableReporting = value;
        }

        /// <summary>
        /// Gets or sets duration between reports.
        /// </summary>
        public float ReportDelay
        {
            get => _transformChangeReporter.ReportDelay;
            set => _transformChangeReporter.ReportDelay = value;
        }

        private ChangeReporter _transformChangeReporter;
        
        private void Awake()
        {
            _transformChangeReporter = new ChangeReporter(_reportDelay, _enableReporting);
        }
        
        /// <summary>
        /// Invokes <see cref="TransformChanged"/> event.
        /// </summary>
        private void Update()
        {
            var tempTransform = transform;
            if (_transformChangeReporter.ReportChangeInIntervals(tempTransform.hasChanged, Time.deltaTime))
            {
                tempTransform.hasChanged = false;
                TransformChanged?.Invoke(new TransformChangedEventArgs { Transform = tempTransform });
            }
        }
    }
}
