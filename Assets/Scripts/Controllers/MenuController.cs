using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Canvas objects")]
    /// <summary> Displayed brush name </summary>
    [SerializeField]
    Text brushNameDisplayTXT;
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

    public void OnExit()
    {
        Application.Quit();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }
}
