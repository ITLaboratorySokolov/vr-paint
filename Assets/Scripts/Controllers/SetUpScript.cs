using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

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
    /// <summary> Name of client </summary>
    [SerializeField]
    private StringVariable clientName;
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConectionController serverConnection;
    /// <summary> Manual connection reset action reference </summary>
    [SerializeField]
    InputActionReference resetAction = null;

    [Header("Room configuration")]
    [SerializeField]
    RoomController roomController;
    [SerializeField]
    Vec2FVariable roomSize;

    /// <summary>
    /// Set up configuration before application starts
    /// -  set room size
    /// /// </summary>
    private void Awake()
    {
        pathToConfig = Application.dataPath + "/../config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        // ReadConfig();
        roomController.SetRoomSize(roomSize.Value.x, roomSize.Value.y);

        if (resetAction != null)
            resetAction.action.performed += ResetSetUp;
    }

    /// <summary>
    /// Read config file
    /// </summary>
    private void ReadConfig()
    {
        if (File.Exists(pathToConfig))
        {
            Debug.Log("Loading config file...");
            string[] lines = File.ReadAllLines(pathToConfig);
            if (lines.Length >= 2)
            {
                // server URL
                clientName.Value = lines[0].Trim();
                serverUrl.Value = lines[1].Trim();
            }

            if (lines.Length >= 3)
            {
                // Size of room
                var size = lines[2].Trim().Split(",");
                float x, z;
                float.TryParse(size[0].Trim(), out x);
                float.TryParse(size[1].Trim(), out z);
                roomController.SetRoomSize(x, z);
            }
        }
    }

    /// <summary>
    /// Reset configuration
    /// </summary>
    /// <param name="ctx"></param>
    public void ResetSetUp(InputAction.CallbackContext ctx)
    {
        Debug.Log("Reseting configuration!");
        ReadConfig();
        serverConnection.ResetConnection();
    }
}
