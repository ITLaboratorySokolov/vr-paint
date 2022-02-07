using UnityEngine;

public class Brush
{
    string name;
    float width;
    Color color;
    Texture2D texture;

    public float Width { get => width; set => width = value; }
    public Color Color { get => color; set => color = value; }
    public Texture2D Texture { get => texture; set => texture = value; }
    public string Name { get => name; set => name = value; }
}
