using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all painting/drawing instruments
/// </summary>
public class IDrawInstrument
{
    /// <summary>
    /// Drawing instrument name
    /// </summary>
    string name;
    public string Name { get => name; set => name = value; }

    /// <summary>
    /// Drawing instrument width
    /// </summary>
    float width;
    public float Width { get => width; set => width = value; }
}
