using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Class controlling the brush palette menu </summary>
public class MenuCanvasController : MonoBehaviour
{
    [Header("Canvas objects")]
    /// <summary> Displayed brush name </summary>
    public Text brushNameDisplayTXT;
    /// <summary> Color picker </summary>
    public FlexibleColorPicker picker;
    /// <summary> Width slider </summary>
    public Slider widthSlider;

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
        String hex = ColorUtility.ToHtmlStringRGB(c);

        Debug.Log(b.Name);
        Debug.Log(hex);
        Debug.Log(picker);

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
}
