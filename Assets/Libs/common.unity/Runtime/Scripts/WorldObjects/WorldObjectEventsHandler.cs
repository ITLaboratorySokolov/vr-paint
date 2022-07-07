using System;
using UnityEngine;
using UnityEngine.Assertions;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Attributes;
using ZCU.TechnologyLab.Common.Unity.Connections;
using ZCU.TechnologyLab.Common.Unity.Utility.Events;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.WorldObjects
{
    /// <summary>
    /// Class reports local changes of world objects to a technology lab server.
    /// </summary>
    public class WorldObjectEventsHandler : MonoBehaviour
    {
        [Header("Networking")]
        [SerializeField]
        [HelpBox("Server Data Connection and Server Session Connection have to be assigned.", HelpBoxAttribute.MessageType.Warning, true)]
        private ServerDataConnectionWrapper serverDataConnection;

        [SerializeField]
        private ServerSessionConnectionWrapper serverSessionConnection;

        private void OnValidate()
        {
            Assert.IsNotNull(this.serverDataConnection, "Server Data Connection was null.");
            Assert.IsNotNull(this.serverSessionConnection, "Server Session Connection was null.");
        }

        /// <summary>
        /// Assigns event handlers that sends updates to a technology lab server whenever an object is changed.
        /// After this call the object will be automatically updated on a server.
        /// </summary>
        /// <param name="worldObject">The object.</param>
        public void AssignEventHandlers(GameObject worldObject)
        {
            IPropertiesManager propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
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
            IPropertiesManager propertiesManager = WorldObjectUtils.GetPropertiesManager(worldObject);
            propertiesManager.PropertiesChanged -= PropertiesManager_PropertiesChanged;

            var transformReport = worldObject.GetComponent<ReportTransformChange>();
            if (transformReport != null)
            {
                transformReport.TransformChanged -= TransformReport_TransformChanged;
            }
        }

        private async void TransformReport_TransformChanged(object sender, TransformChangedEventArgs e)
        {
            try
            {
                Debug.Log($"Transform of {e.Transform.gameObject.name} changed. Update server");

                var transformDto = new WorldObjectTransformDto
                {
                    ObjectName = e.Transform.gameObject.name,
                    Position = new RemoteVectorDto
                    {
                        X = e.Transform.position.x,
                        Y = e.Transform.position.y,
                        Z = e.Transform.position.z
                    },
                    Rotation = new RemoteVectorDto
                    {
                        X = e.Transform.rotation.eulerAngles.x,
                        Y = e.Transform.rotation.eulerAngles.y,
                        Z = e.Transform.rotation.eulerAngles.z
                    },
                    Scale = new RemoteVectorDto
                    {
                        X = e.Transform.localScale.x,
                        Y = e.Transform.localScale.y,
                        Z = e.Transform.localScale.z
                    }
                };

                await this.serverSessionConnection.Session.TransformWorldObjectAsync(transformDto);
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

                await this.serverDataConnection.Data.UpdateWorldObjectPropertiesAsync(e.ObjectName, propertiesDto);
            }
            catch (Exception ex)
            {
                Debug.Log("Unable to update property on a server.", this);
                Debug.LogException(ex);
            }
        }
    }
}
