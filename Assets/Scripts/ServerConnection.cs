﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Unity.Connections.Session;
using ZCU.TechnologyLab.Common.Connections.Session;
using ZCU.TechnologyLab.Common.Connections;
using ZCU.TechnologyLab.Common.Unity.Connections.Data;
using ZCU.TechnologyLab.Common.Unity.WorldObjects;

// TODO does reconnect work - was reworked

/// <summary>
/// Class that manages connection to server
/// - connects to server
/// - 4 times per second sends updates of the screen
/// - disconnects from server
/// </summary>
public class ServerConnection : MonoBehaviour
{
    [Header("Connection")]
    /// <summary> Connection to server </summary>
    [SerializeField]
    ServerSessionConnection connection;
    /// <summary> Data connection to server </summary>
    ServerDataConnection dataConnection;
    /// <summary> Session </summary>
    [SerializeField]
    SignalRSessionWrapper session;
    /// <summary> Data session </summary>
    [SerializeField]
    RestDataClientWrapper dataSession;
    /// <summary> Synchronization call has been finished </summary>
    internal bool syncCallDone;

    [Header("World object managers")]
    /// <summary> World object manager </summary>
    [SerializeField]
    WorldObjectManager woManager;
    /// <summary> Paint controller </summary>
    [SerializeField]
    PaintingController paintCont;
    /// <summary> Number of lines already on server </summary>
    internal int serverLines;

    [Header("Actions")]
    /// <summary> Action performed upon Start </summary>
    [SerializeField]
    UnityEvent actionStart = new UnityEvent();
    /// <summary Action performed upon Destroy </summary>
    [SerializeField]
    UnityEvent actionEnd = new UnityEvent();

    [Header("Hand objects")]
    [SerializeField]
    bool leftHanded;
    /// <summary> Hand displayed when online </summary>
    [SerializeField]
    GameObject[] handOnline;
    /// <summary> Hand displayed when offline </summary>
    [SerializeField]
    GameObject[] handOffline;
    /// <summary> Text displayed when offline </summary>
    [SerializeField]
    GameObject[] textOffline;

    /// <summary>
    /// Performes once upon start
    /// - creates instances of needed local classes
    /// - calls action actionStart
    /// </summary>
    private void Start()
    {
        actionStart.Invoke();

        connection = new ServerSessionConnection(session);
        dataConnection = new ServerDataConnection(dataSession);

        handOnline[0].SetActive(false);
        handOnline[1].SetActive(false);

        handOffline[0].SetActive(true);
        handOffline[1].SetActive(true);
        
        if (leftHanded)
        {
            textOffline[0].SetActive(true);
            textOffline[1].SetActive(false);
        }
        else
        {
            textOffline[0].SetActive(false);
            textOffline[1].SetActive(true);
        }

        // TODO this exists bc i cannot connect to server in lab
        textOffline[0].SetActive(false);
        textOffline[1].SetActive(false);
        handOnline[0].SetActive(true);
        handOnline[1].SetActive(true);
        handOffline[0].SetActive(false);
        handOffline[1].SetActive(false);
        syncCallDone = true;

        // TODO + edited controller left hand - grip pressed for picking colours
        // TODO rotation is weird now -> needs fix to rotate around camera

    }

    /// <summary>
    /// Called when automatic connection to server fails
    /// - attempts to restart connection to server
    /// </summary>
    public void ConnectionFailed()
    {
        Debug.Log("Launching restart procedure");
        StartCoroutine(RestartConnection());
    }

    /// <summary>
    /// Restarting procedure
    /// - creates a minimum 5s delay
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartConnection()
    {
        yield return new WaitForSeconds(5);
        actionStart.Invoke();
    }

    /// <summary>
    /// Reset connection to server
    /// </summary>
    public void ResetConnection()
    {
        syncCallDone = false;
    }

    /// <summary>
    /// Called when successfully connected to server
    /// </summary>
    public void ConnectedToServer()
    {
        Debug.Log("Connected to server");
        StartCoroutine(SyncCall());

        textOffline[0].SetActive(false);
        textOffline[1].SetActive(false);

        handOnline[0].SetActive(true);
        handOnline[1].SetActive(true);

        handOffline[0].SetActive(false);
        handOffline[1].SetActive(false);
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

    /// <summary>
    /// Get objects from server
    /// - filter lines and display them
    /// </summary>
    private async void GetObjectsAsync()
    {
        try
        {
            // Get all objects
            IEnumerable<WorldObjectDto> objs = await dataConnection.GetAllWorldObjectsAsync();
            List<int> l = new List<int>();

            // Go through the names and parse
            foreach (WorldObjectDto obj in objs)
            {
                string n = obj.Name;
                Debug.Log(n);

                // Filter out lines
                serverLines = 0;
                if (n.StartsWith("Line"))
                {
                    string num = n.Substring(4, n.Length - 4);
                    int numP = 0;
                    int.TryParse(num, out numP);
                    l.Add(numP);

                    paintCont.AddServerLine(obj);
                }

            }

            for (int i = 0; i < l.Count; i++)
                if (l[i] > serverLines)
                    serverLines = l[i];
        }
        catch (Exception e)
        {
            Debug.LogError("Cannot sync call");
            Debug.LogError(e.Message);
        }
        syncCallDone = true;
        Debug.Log("Sync call done");
    }

    

    /// <summary>
    /// Action called on ending the application
    /// </summary>
    public void OnDestroy()
    {
        actionEnd.Invoke();
    }
}
