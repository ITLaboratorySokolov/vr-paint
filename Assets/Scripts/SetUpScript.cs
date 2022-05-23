using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
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

    /// <summary>
    /// Set up configuration before application starts
    /// - read from config min and max recorded depth, horizontal and vertical pan, zoom and server url
    /// </summary>
    private void Awake()
    {
        pathToConfig = "./config.txt"; // Directory.GetCurrentDirectory() + "\\config.txt";
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
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


}
