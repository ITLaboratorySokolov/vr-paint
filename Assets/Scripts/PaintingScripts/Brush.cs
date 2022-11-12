using System.Xml.Serialization;
using UnityEngine;

/// <summary> Class representing a brush </summary>
public class Brush : IDrawInstrument
{
    /// <summary>
    /// Brush color
    /// </summary>
    Color color;
    public Color Color { get => color; set => color = value; }

    /// <summary>
    /// Brush texture
    /// </summary>
    Texture2D texture;
    [XmlIgnore]
    public Texture2D Texture { get => texture; set => texture = value; }

    /// <summary>
    /// Width curve
    /// </summary>
    AnimationCurve widthCurve;
    public AnimationCurve WidthCurve { get => widthCurve; set => widthCurve = value; }
}
