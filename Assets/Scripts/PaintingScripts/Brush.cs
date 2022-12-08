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

    /// <summary> Width modifiers, values 0-1 - percentage of width at constant timesteps </summary>
    float[] widthModifier;
    public float[] WidthModifier { get => widthModifier; set => widthModifier = value; }

    /// <summary> Time per one iteration of width </summary>
    float timePerIter;
    public float TimePerIter { get => timePerIter; set => timePerIter = value; }

}
