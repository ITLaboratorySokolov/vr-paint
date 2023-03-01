using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    GameObject exitPanel;
    [SerializeField]
    GameObject controlsPanel;

    public void ToggleExitCanvas(bool val)
    {
        exitPanel.SetActive(val);
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void ToggleControlsPanel(bool val)
    {
        controlsPanel.SetActive(val);
    }
}
