using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaintingTest : MonoBehaviour
{
    public InputActionReference toggleRef = null;
    public GameObject spawn;
    public Transform controller;

    public GameObject simpleLine;
    public Transform lineParent;
    internal LineRenderer currLine;

    bool tracking = false;
    List<Vector3> path;

    private void Awake()
    {
        toggleRef.action.started += ActivatePaint;
    }

    private void OnDestroy()
    {
        toggleRef.action.started -= ActivatePaint;
    }

    private void ActivatePaint(InputAction.CallbackContext ctx)
    {
        tracking = true;
        path = new List<Vector3>();

        GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
        currLine = o.GetComponent<LineRenderer>();
        
        currLine.SetPosition(0, controller.position);
        currLine.SetPosition(1, controller.position);
    }

    private void DisablePaint()
    {
        tracking = false;

        Debug.Log(path.Count);
    }

    private void Toggle(InputAction.CallbackContext ctx)
    {
        Instantiate(spawn, controller.position, controller.rotation);

        bool isActive = !gameObject.activeSelf;
        // gameObject.SetActive(isActive);
        Debug.Log("Toggle");
    }

    // Start is called before the first frame update
    void Start()
    {

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
            path.Add(controller.position);
            currLine.positionCount++;
            currLine.SetPosition(currLine.positionCount - 1, controller.position);
        }

        if (!tracking && path != null)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i-1], path[i]);
            }
        }
    }

}
