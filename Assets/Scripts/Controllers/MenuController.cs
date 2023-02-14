using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Canvas objects")]
    /// <summary> Displayed brush name </summary>
    public Text brushNameDisplayTXT;
    /// <summary> Color picker </summary>
    public FlexibleColorPicker picker;
    /// <summary> Width slider </summary>
    public Slider widthSlider;

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
}
