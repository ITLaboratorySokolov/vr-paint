using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

public class MenuController : MonoBehaviour
{
    [Header("Canvas objects")]
    /// <summary> Displayed brush name </summary>
    [SerializeField]
    Text brushNameDisplayTXT;
    [SerializeField]
    TMP_Text connectionTXT;
    /// <summary> Color picker </summary>
    [SerializeField]
    FlexibleColorPicker picker;
    /// <summary> Width slider </summary>
    [SerializeField]
    Slider widthSlider;
    /// <summary> Exit panel </summary>
    [SerializeField]
    GameObject exitPanel;
    /// <summary> Controls panel </summary>
    [SerializeField]
    GameObject controlsPanel;

    [Header("Scripts")]
    public LanguageController langController;

    /// <summary>
    /// Set brush values to display currently selected brush
    /// </summary>
    /// <param name="b"> Brush </param>
    public void SwitchBrushValues(Brush b)
    {
        // Set width
        widthSlider.value = b.Width / 0.1f;

        // Set current color to picker
        Color c = b.Color;
        string hex = ColorUtility.ToHtmlStringRGB(c);

        picker.FinishTypeHex(hex);

        // Set name
        brushNameDisplayTXT.text = b.Name;
    }

    /// <summary>
    /// Switch to eraser
    /// </summary>
    public void SwitchToEraser(Eraser b)
    {
        // Set width
        widthSlider.value = b.Width / 0.1f;

        // Set name
        brushNameDisplayTXT.text = b.Name;
    }

    public void SwitchLanguage()
    {
        langController.SwapLanguage();
    }

    public void ToggleExitCanvas(bool val)
    {
        exitPanel.SetActive(val);
    }

    public void OnExit(ServerConectionController scc)
    {
        scc.OnExit();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }

    /// <summary>
    /// Display connection status
    /// </summary>
    /// <param name="connected"> Is connected to server </param>
    public void SetConnection(SignalRSessionWrapper session) // bool connected)
    {
        Debug.Log(session.State.ToString());

        string state = langController.GetSessionStateString(session.State);

        if (session.State == SessionState.Connected)
            ChangeConnection(state, Color.green);
        else if (session.State == SessionState.Reconnecting)
            ChangeConnection(state, Color.yellow);
        else
            ChangeConnection(state, Color.red);
    }

    /// <summary>
    /// Change displayed connection status
    /// </summary>
    /// <param name="msg"> Message </param>
    /// <param name="c"> Colour of text </param>
    public void ChangeConnection(string msg, Color c)
    {
        connectionTXT.text = msg;
        connectionTXT.color = c;
    }
}
