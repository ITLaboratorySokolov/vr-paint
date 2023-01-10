using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

// TODO path to config file

/// <summary>
/// Script managing the set up of the application
/// - reads config file
/// </summary>
public class SetUpScript : MonoBehaviour
{
    [Header("Config")]
    /// <summary> Path to config file </summary>
    string pathToConfig;

    [Header("Server connection")]
    /// <summary> Server url </summary>
    [SerializeField]
    private StringVariable serverUrl;
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConnection serverConnection;
    /// <summary> Manual connection reset action reference </summary>
    [SerializeField]
    InputActionReference resetAction = null;

    [Header("Room configuration")]
    [SerializeField]
    RoomController roomController;

    [Header("Hands")]
    /// <summary> Hand displayed when online </summary>
    [SerializeField]
    GameObject[] handCanvas;
    [SerializeField]
    GameObject[] handLaser;
    [SerializeField]
    GameObject[] game;

    /// <summary>
    /// Set up configuration1before application starts
    /// - read from config min and max recorded depth, horizontal and vertical pan, zoom and server url
    /// </summary>
    private void Awake()
    {
        pathToConfig = Application.dataPath + "/../config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();

        if (resetAction != null)
            resetAction.action.performed += ResetSetUp;
    }

    private void Start()
    {

    }

    private void ReadConfig()
    {
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 1)
            {
                // server URL
                serverUrl.Value = lines[0].Trim();
            }

            if (lines.Length >= 2)
            {
                // Size of room
                var size = lines[1].Trim().Split(",");
                float x, z;
                float.TryParse(size[0].Trim(), out x);
                float.TryParse(size[1].Trim(), out z);
                roomController.SetRoomSize(x, z);
            }
        }
    }

    public void ResetSetUp(InputAction.CallbackContext ctx)
    {
        Debug.Log("Reseting configuration!");
        ReadConfig();
        serverConnection.ResetConnection();
    }
}
