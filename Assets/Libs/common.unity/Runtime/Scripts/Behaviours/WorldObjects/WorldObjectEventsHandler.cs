using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Repository.Server;
using ZCU.TechnologyLab.Common.Unity.Models.Attributes;
using ZCU.TechnologyLab.Common.Unity.Models.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects
{
    /// <summary>
    /// Class reports local changes of world objects to a technology lab server.
    /// </summary>
    public class WorldObjectEventsHandler : MonoBehaviour
    {
        [Header("Networking")]
        [SerializeField]
        [HelpBox("Server Data Connection and Server Session Connection have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        [FormerlySerializedAs("serverDataAdapter")]
        private ServerDataAdapterWrapper _serverDataAdapter;

        [SerializeField]
        [FormerlySerializedAs("serverSessionAdapter")]
        private ServerSessionAdapterWrapper _serverSessionAdapter;

        private void OnValidate()
        {
            Assert.IsNotNull(_serverDataAdapter, "Server Data Connection was null.");
            Assert.IsNotNull(_serverSessionAdapter, "Server Session Connection was null.");
        }

        /// <summary>
        /// Assigns event handlers that sends updates to a technology lab server whenever an object is changed.
        /// After this call the object will be automatically updated on a server.
        /// </summary>
        /// <param name="worldObject">The object.</param>
        public void AssignEventHandlers(GameObject worldObject)
        {
            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
            propertiesManager.PropertiesChanged += PropertiesManager_PropertiesChanged;

            var transformReport = worldObject.GetComponent<ReportTransformChange>();
            if (transformReport != null)
            {
                transformReport.TransformChanged += TransformReport_TransformChanged;
            }
        }

        /// <summary>
        /// Removes event handlers that sends updates to a technology lab server whenever an object is changed.
        /// After this call object won't be automatically updated anymore.
        /// </summary>
        /// <param name="worldObject">The object.</param>
        public void RemoveEventHandlers(GameObject worldObject)
        {
            var propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
            propertiesManager.PropertiesChanged -= PropertiesManager_PropertiesChanged;

            var transformReport = worldObject.GetComponent<ReportTransformChange>();
            if (transformReport != null)
            {
                transformReport.TransformChanged -= TransformReport_TransformChanged;
            }
        }

        private async void TransformReport_TransformChanged(TransformChangedEventArgs e)
        {
            try
            {
                Debug.Log($"Transform of {e.Transform.gameObject.name} changed. Update server");

                var position = e.Transform.position;
                var rotation = e.Transform.rotation;
                var localScale = e.Transform.localScale;
                
                var transformDto = new WorldObjectTransformDto
                {
                    ObjectName = e.Transform.gameObject.name,
                    Position = new RemoteVectorDto
                    {
                        X = position.x,
                        Y = position.y,
                        Z = position.z
                    },
                    Rotation = new RemoteVectorDto
                    {
                        X = rotation.eulerAngles.x,
                        Y = rotation.eulerAngles.y,
                        Z = rotation.eulerAngles.z
                    },
                    Scale = new RemoteVectorDto
                    {
                        X = localScale.x,
                        Y = localScale.y,
                        Z = localScale.z
                    }
                };

                await _serverSessionAdapter.TransformWorldObjectAsync(transformDto);
            }
            catch (Exception ex)
            {
                Debug.Log("Unable to update transform on a server.", this);
                Debug.LogException(ex);
            }
        }

        private async void PropertiesManager_PropertiesChanged(object sender, PropertiesChangedEventArgs e)
        {
            try
            {
                Debug.Log($"Properties of {e.ObjectName} changed. Update server");

                var propertiesDto = new WorldObjectPropertiesDto
                {
                    Properties = e.Properties
                };

                await _serverDataAdapter.UpdateWorldObjectPropertiesAsync(e.ObjectName, propertiesDto);
            }
            catch (Exception ex)
            {
                Debug.Log("Unable to update property on a server.", this);
                Debug.LogException(ex);
            }
        }
    }
}