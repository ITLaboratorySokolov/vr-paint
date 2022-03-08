using UnityEngine;

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
    public Texture2D Texture { get => texture; set => texture = value; }

}
