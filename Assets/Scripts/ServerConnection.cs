using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;

// TODO retry connection
// TODO yells on close that screen capture cannot be done outside of playmode!

/// <summary>
/// Class that manages connection to server
/// - connects to server
/// - 4 times per second sends updates of the screen
/// - disconnects from server
/// </summary>
public class ServerConnection : MonoBehaviour
{
    /// <summary> Countdown to next image send </summary>
    private double timeToSnapshot;

    /// <summary> Connection to server </summary>
    [SerializeField]
    ServerSessionConnection connection;
    ServerDataConnection dataConnection;
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    [SerializeField]
    RestDataClientWrapper dataSession;

    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    /// <summary> Bitmap serializer </summary>
    BitmapSerializer serializer;
    /// <summary> World object DTO for screenshot to be sent to server </summary>
    WorldObjectDto wod;
    /// <summary> Synchronization call has been finished </summary>
    bool syncCallDone;

    Dictionary<string, byte[]> properties;
    Texture2D scaled;
    byte[] data;
    System.Diagnostics.Stopwatch stopWatch;

    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        serializer = new BitmapSerializer();
        connection = new ServerSessionConnection(session);
        dataConnection = new ServerDataConnection(dataSession);
        //session.StartSession();
        actionStart.Invoke();

        StartCoroutine(SyncCall());
    }

    /// <summary>
    /// Starting synchronization call
    /// </summary>
    /// <returns> IEnumerator </returns>
    IEnumerator SyncCall()
    {
        yield return new WaitUntil(() => session.SessionState == SessionState.Connected);
        GetObjectsAsync();
    }

    private async void GetObjectsAsync()
    {
        // TODO find out how many lines are there on server / whats the highest ID

        try
        {
            // Get all objects

            // go through the names and parse
        }
        catch
        {
            Debug.Log("Init sent to server");
        }
        syncCallDone = true;
        Debug.Log("Sync call done");
    }

    /// <summary>
    /// Process incoming objects from server
    /// </summary>
    /// <param name="l"> List of objects </param>
    private void ProcessObjects(List<WorldObjectDto> l)
    {
        // TODO Go through incoming objects and parse names

        bool present = false;

        // Look through l for "FlyKiller"
        for (int i = 0; i < l.Count; i++)
            if (l[i].Name == wod.Name)
                present = true;

        Debug.Log("Present? " + present);

        syncCallDone = true;
    }

    public void OnDestroy()
    {
        //session.StopSession();
        actionEnd.Invoke();
    }
}
