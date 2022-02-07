using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaintingTest : MonoBehaviour
{
    public InputActionReference toggleRef = null;
    public InputActionReference switchRef = null;
    public GameObject spawn;
    public Transform controller;

    bool tracking = false;

    [Header("Line Rendering")]
    public GameObject simpleLine;
    public Transform lineParent;
    internal LineRenderer currLine;
    List<Vector3> path;

    [Header("Brushes")]
    public Text brushNameTXT;
    Brush[] brushes;
    public int numberOfBrushes = 2;
    Color selectedColor;
    int currentBrush;

    private void Awake()
    {
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 8, Color = Color.red, Texture = null, Name = "Brush01" };
        brushes[1] = new Brush() { Width = 8, Color = Color.black, Texture = null, Name = "Brush02" };

        toggleRef.action.started += ActivatePaint;
        switchRef.action.started += SwitchBrush;
    }

    void Start()
    {
        
    }

    private void OnDestroy()
    {
        toggleRef.action.started -= ActivatePaint;
        switchRef.action.started -= SwitchBrush;
    }

    private void SwitchBrush(InputAction.CallbackContext obj)
    {
        Debug.Log(currentBrush);
        currentBrush++;
        currentBrush = currentBrush % numberOfBrushes; // index of current brush
        brushNameTXT.text = brushes[currentBrush].Name;
    }

    private void ActivatePaint(InputAction.CallbackContext ctx)
    {
        tracking = true;
        path = new List<Vector3>();

        GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
        currLine = o.GetComponent<LineRenderer>();
        currLine.material.SetColor("_Color", brushes[currentBrush].Color);
        // currLine.widthCurve = brushes[currentBrush].Width;

        currLine.SetPosition(0, controller.position);
        currLine.SetPosition(1, controller.position);
    }

    private void DisablePaint()
    {
        tracking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleRef.action.WasReleasedThisFrame())
        {
            DisablePaint();
        }

        if (tracking)
        {
            currLine.positionCount++;
            currLine.SetPosition(currLine.positionCount - 1, controller.position);
        }
    }

    public void ColorChange(Color c)
    {
        if (brushes != null && brushes[currentBrush] != null) 
            brushes[currentBrush].Color = c;
    }

}
