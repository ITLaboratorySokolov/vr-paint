using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

/// <summary>
/// Script used to controll the intro screen and set configuration of the application
/// </summary>
public class ConfigController : MonoBehaviour
{
    [Header("Config")]
    string pathToConfig;

    [SerializeField]
    string nextScene;
    [SerializeField]
    ConfigLanguageController langController;

    [Header("Variables")]
    [SerializeField]
    private StringVariable serverUrl;
    [SerializeField]
    private StringVariable clientName;
    [SerializeField]
    private Vec2FVariable roomSize;

    [Header("Display Values")]
    [SerializeField()]
    TMP_InputField nameFLD;
    [SerializeField()]
    TMP_InputField urlFLD;

    [Header("GameObjects")]
    [SerializeField]
    GameObject controlsPanel;
    
    [Header("ErrorIcons")]
    [SerializeField]
    GameObject errorNM;
    [SerializeField]
    GameObject errorURL;

    // Start is called before the first frame update
    private void Start()
    {
        pathToConfig = Directory.GetCurrentDirectory() + "\\config.txt"; // 
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();
        langController.SwapLabels();
        langController.SetControls();

        DisplayValues();
    }

    /// <summary>
    /// Display values on screen
    /// </summary>
    private void DisplayValues()
    {
        nameFLD.text = clientName.Value;
        urlFLD.text = serverUrl.Value;
    }

    /// <summary>
    /// On Play button clicked
    /// </summary>
    public void Play()
    {
        serverUrl.Value = urlFLD.text.Trim();
        clientName.Value = nameFLD.text.Trim();

        urlFLD.text = serverUrl.Value;
        nameFLD.text = clientName.Value;

        // if no client name set
        bool noClient = false;
        if (clientName.Value == null || clientName.Value.Length == 0)
            noClient = true;
        errorNM.SetActive(noClient);

        // if no server url set
        bool noUrl = false;
        if (serverUrl.Value == null || serverUrl.Value.Length == 0)
            noUrl = true;
        errorURL.SetActive(noUrl);

        if (noUrl || noClient)
            return;

        SceneManager.LoadScene(nextScene);
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
                roomSize.Value = new Vector2(x, z);
            }
        }
    }

    /// <summary>
    /// Filter username
    /// - only a-zA-Z0-9_- allowed
    /// </summary>
    public void FilterName()
    {
        nameFLD.text = Regex.Replace(nameFLD.text, "[^a-zA-Z0-9_-]+", "", RegexOptions.Compiled);
    }

    /// <summary>
    /// On Exit button clicked
    /// </summary>
    public void OnExit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggle panel displaying app controls
    /// </summary>
    /// <param name="val"> True for panel on, false for panel off </param>
    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }
}
