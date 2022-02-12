using UnityEngine;

public class Brush
{
    /// <summary>
    /// Brush name
    /// </summary>
    string name;
    public string Name { get => name; set => name = value; }
 
    /// <summary>
    /// Brush width
    /// </summary>
    float width;
    public float Width { get => width; set => width = value; }
    
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
