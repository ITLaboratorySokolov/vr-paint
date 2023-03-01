using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

public class ConfigController : MonoBehaviour
{
    [Header("Config")]
    /// <summary> Path to config file </summary>
    string pathToConfig;

    [SerializeField]
    string nextScene;
    [SerializeField]
    ConfigLanguageController langController;

    [Header("Variables")]
    /// <summary> Server url </summary>
    [SerializeField]
    private StringVariable serverUrl;
    /// <summary> Name of client </summary>
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


    // Start is called before the first frame update
    private void Start()
    {
        pathToConfig = Directory.GetCurrentDirectory() + "\\config.txt"; // 
        Debug.Log(pathToConfig);

        // Set culture -> doubles are written with decimal dot
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        ReadConfig();
        langController.SwapLabels();

        DisplayValues();
    }

    private void DisplayValues()
    {
        nameFLD.text = clientName.Value;
        urlFLD.text = serverUrl.Value;
    }

    public void Play()
    {
        serverUrl.Value = urlFLD.text.Trim();
        clientName.Value = nameFLD.text.Trim();

        SceneManager.LoadScene(nextScene);
    }

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

    public void OnExit()
    {
        Application.Quit();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }
}
