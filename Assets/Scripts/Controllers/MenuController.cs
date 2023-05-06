using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZCU.TechnologyLab.Common.Connections.Client.Session;
using ZCU.TechnologyLab.Common.Unity.Behaviours.Connections.Client.Session;

/// <summary>
/// Script controlling the palette menu
/// </summary>
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
    /// <summary> Save scenes panel </summary>
    [SerializeField]
    GameObject savePanel;

    [Header("Scripts")]
    /// <summary> Language controller </summary>
    [SerializeField]
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
    /// <param name="b"> Eraser </param>
    public void SwitchToEraser(Eraser b)
    {
        // Set width
        widthSlider.value = b.Width / 0.1f;

        // Set name
        brushNameDisplayTXT.text = b.Name;
    }

    /// <summary>
    /// Switch language
    /// </summary>
    public void SwitchLanguage()
    {
        langController.SwapLanguage();
    }

    /// <summary>
    /// Toggle exit canvas
    /// </summary>
    /// <param name="val"> True if exit canvas on, false if off </param>
    public void ToggleExitCanvas(bool val)
    {
        exitPanel.SetActive(val);
    }

    /// <summary>
    /// Toggle save canvas
    /// </summary>
    /// <param name="val"> True if save canvas on, false if off </param>
    public void ToggleSaveCanvas(bool val)
    {
        savePanel.SetActive(val);
    }

    /// <summary>
    /// On exit button clicked
    /// </summary>
    /// <param name="scc"></param>
    public void OnExit(ServerConectionController scc)
    {
        scc.OnExit();
    }

    /// <summary>
    /// Toggle controls canvas
    /// </summary>
    /// <param name="val"> True if control canvas on, false if off </param>
    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }

    /// <summary>
    /// Process session state
    /// </summary>
    /// <param name="session"> Active session </param>
    public void SetConnection(SignalRSessionWrapper session) 
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
