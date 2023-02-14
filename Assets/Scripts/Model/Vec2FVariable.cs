using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A string variable that can be saved to assets.
/// </summary>
[CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab/Variables/Vec2FVariable")]
public class Vec2FVariable : ScriptableObject
{
    [SerializeField]
    private Vector2 value;

    /// <summary>
    /// Value of the variable.
    /// </summary>
    public Vector2 Value
    {
        get => this.value;
        set => this.value = value;
    }
}