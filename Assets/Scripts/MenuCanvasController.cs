using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasController : MonoBehaviour
{
    [Header("Canvas objects")]
    public Text brushNameDisplayTXT;
    public FlexibleColorPicker picker;
    public Slider widthSlider;

    /// <summary>
    /// Set brush values to display currently selected brush
    /// </summary>
    /// <param name="b"> Brush </param>
    public void SwitchBrushValues(Brush b)
    {
        // set width
        widthSlider.value = b.Width / 0.1f;

        // set current color to picker
        Color c = b.Color;
        String hex = ColorUtility.ToHtmlStringRGB(c);
        picker.FinishTypeHex(hex);

        // set name
        brushNameDisplayTXT.text = b.Name;
    }

    /// <summary>
    /// Set brush to eraser
    /// </summary>
    public void SwitchToEraser(Eraser b)
    {
        // set width
        widthSlider.value = b.Width / 0.1f;

        // set name
        brushNameDisplayTXT.text = b.Name;
    }
}
