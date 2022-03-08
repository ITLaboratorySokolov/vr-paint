using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO - what if you switch to eraser during painting

public class PaintingController : MonoBehaviour
{
    [Header("Action input references")]
    public InputActionReference paintRef = null;
    public InputActionReference switchRef = null;
    public InputActionReference eraserRef = null;
    public InputActionReference moveRef = null;
    public InputActionReference rotRef = null;

    [Header("Game objects")]
    public Transform rig;
    public MenuCanvasController canvas;
    public Transform controller;
    bool paintingOn = false;
    public Transform hand;

    [Header("Controlls")]
    public int speed;
    bool isEraser;

    [Header("Line Rendering")]
    public GameObject simpleLine;
    public Transform lineParent;
    internal LineRenderer currLine;

    [Header("Brushes")]
    public int numberOfBrushes = 2;
    public Texture2D[] textures;
    Brush[] brushes;
    int currentBrush;

    /// <summary>
    /// Register actions on object awake, set up brushes
    /// </summary>
    private void Awake()
    {
        // manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.red, Texture = textures[0], Name = "Brush01" };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = textures[1], Name = "Brush02" };

        // register actions
        paintRef.action.started += ActivatePaint;
        switchRef.action.started += SwitchBrush;
        eraserRef.action.started += ToggleEraser;

        // display brush values on canvas
        currentBrush = 0;
        canvas.SwitchBrushValues(brushes[currentBrush]);
    }

    // Update is called once per frame
    void Update()
    {
        // movement and rotation
        MoveAndRotateRig();

        // stop drawing on released button
        if (paintRef.action.WasReleasedThisFrame())
            DisablePaint();

        // paint
        if (paintingOn)
        {
            Paint();
        }
    }

    /// <summary>
    /// Unregister actions on object destroyed
    /// </summary>
    private void OnDestroy()
    {
        paintRef.action.started -= ActivatePaint;
        switchRef.action.started -= SwitchBrush;
        eraserRef.action.started -= ToggleEraser;

    }

    /// <summary>
    /// Toggle eraser brush
    /// </summary>
    /// <param name="obj"></param>
    private void ToggleEraser(InputAction.CallbackContext obj)
    {
        if (paintingOn)
            return;

        isEraser = !isEraser;

        // display information on canvas
        if (isEraser)
            canvas.SwitchToEraser();
        else
            canvas.SwitchBrushValues(brushes[currentBrush]);
    }

    /// <summary>
    /// Switch active brush
    /// - set current index
    /// </summary>
    /// <param name="obj"></param>
    private void SwitchBrush(InputAction.CallbackContext obj)
    {
        if (isEraser)
            return;

        currentBrush++;
        currentBrush %= numberOfBrushes;
        canvas.SwitchBrushValues(brushes[currentBrush]);
    }

    /// <summary>
    /// Start painting
    /// - spawn new line and set values according to brush
    /// </summary>
    /// <param name="ctx"> Callback context </param>
    private void ActivatePaint(InputAction.CallbackContext ctx)
    {
        // erase
        if (isEraser)
        {
            Collider[] cs = Physics.OverlapSphere(controller.transform.position, 0.025f);
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i].gameObject.tag == "line")
                    Destroy(cs[i].gameObject);
            }
        }
        // paint
        else
        {
            paintingOn = true;

            // instantiate new line
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);

            // get line renderer
            currLine = o.GetComponent<LineRenderer>();

            // set color
            currLine.material.SetColor("_Color", brushes[currentBrush].Color);

            // set texture - only if the brush has any, otherwise stays the default of the material
            if (brushes[currentBrush].Texture != null)
                currLine.material.SetTexture("_MainTex", brushes[currentBrush].Texture);

            // set line width
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1);
            curve.AddKey(1.0f, 1);
            currLine.widthMultiplier = brushes[currentBrush].Width;
            currLine.widthCurve = curve;

            // set first 2 positions
            currLine.SetPosition(0, controller.position);
            currLine.SetPosition(1, controller.position);
        }
    }

    /// <summary>
    /// Painting = adding another position to line
    /// </summary>
    private void Paint()
    {
        currLine.positionCount++;
        currLine.SetPosition(currLine.positionCount - 1, controller.position);
    }

    /// <summary>
    /// Stop painting
    /// </summary>
    private void DisablePaint()
    {
        if (isEraser)
            return; 

        paintingOn = false;

        Vector3[] points = new Vector3[currLine.positionCount];

        MeshCollider meshCollider = currLine.GetComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        currLine.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Move rig based on input
    /// </summary>
    private void MoveAndRotateRig()
    {
        // movement
        Vector2 v = moveRef.action.ReadValue<Vector2>();
        rig.position += rig.transform.TransformVector(new Vector3(v.x, 0, v.y)) * speed * Time.deltaTime;

        // rotation
        Vector2 w = rotRef.action.ReadValue<Vector2>();
        float angle = (Mathf.Atan2(w.y, w.x));
        if (angle != 0)
            angle = Mathf.Deg2Rad * 90 - angle;

        // cap rotation at max +-90°
        if (Math.Abs(angle) > Mathf.Deg2Rad * 180)
            angle = -Mathf.Deg2Rad * 90;
        else if (Math.Abs(angle) > Mathf.Deg2Rad * 90)
            angle = Mathf.Deg2Rad * 90;

        rig.rotation *= Quaternion.AngleAxis(angle * speed, Vector3.up);
    }

    /// <summary>
    /// Change colour of current brush
    /// </summary>
    /// <param name="c"> Colour </param>
    public void ColorChange(Color c)
    {
        if (isEraser)
            return;

        if (brushes != null && brushes[currentBrush] != null)
            brushes[currentBrush].Color = c;
    }

    /// <summary>
    /// Set width of current brush
    /// </summary>
    /// <param name="f"> Width </param>
    public void SetBrushWidth(float f)
    {
        hand.localScale = new Vector3(0.1f * f, 0.1f * f, 0.1f * f);

        if (isEraser)
            return;

        brushes[currentBrush].Width = f * 0.1f;
    }

}
