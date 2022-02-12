using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColorChange(Color c)
    {
        var cubeRenderer = this.GetComponent<Renderer>();
        cubeRenderer.material.SetColor("_Color", c);
    }
}
