using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Unity.AssetVariables;

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

    [Header("Hand switch")]
    [SerializeField]
    bool leftHanded;
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
        if (leftHanded)
        {
            handCanvas[0].SetActive(false);
            handCanvas[1].SetActive(true);

            handLaser[0].SetActive(true);
            handLaser[1].SetActive(false);

            // TODO needs to swap all references to ServerConnection etc
            // Instantiate(game[0]);
        }
        else
        {
            handCanvas[0].SetActive(true);
            handCanvas[1].SetActive(false);

            handLaser[0].SetActive(false);
            handLaser[1].SetActive(true);

            // Instantiate(game[1]);
        }

        pathToConfig = "./config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();

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
                serverUrl.Value = lines[0].Trim();
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
