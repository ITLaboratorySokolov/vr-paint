using UnityEngine;
using UnityEngine.InputSystem;

// TODO - what if you switch to eraser during painting

/// <summary> Class controlling painting </summary>
public class PaintingController : MonoBehaviour
{
    [Header("Action input references")]
    /// <summary> Painting action </summary>
    public InputActionReference paintRef = null;
    /// <summary> Switch brush action </summary>
    public InputActionReference switchRef = null;
    /// <summary> Switch to eraser action </summary>
    public InputActionReference eraserRef = null;

    [Header("Game objects")]
    /// <summary> Color palette menu canvas controller </summary>
    public MenuCanvasController canvas;
    /// <summary> Painting hand controller grip </summary>
    public Transform controller;
    /// <summary> Is user currently painting </summary>
    bool paintingOn = false;
    /// <summary> Painting hand object </summary>
    public Transform hand;

    [Header("Controlls")]
    /// <summary> Movement speed </summary>
    public int speed;
    /// <summary> Is currently eraser enabled </summary>
    bool isEraser;

    [Header("Line Rendering")]
    /// <summary> Line prefab </summary>
    public GameObject simpleLine;
    /// <summary> Line parent </summary>
    public Transform lineParent;
    /// <summary> Current line </summary>
    internal LineRenderer currLine;

    [Header("Brushes")]
    /// <summary> Number of brushes in system </summary>
    public int numberOfBrushes = 2;
    /// <summary> Textures for brushes </summary>
    public Texture2D[] textures;
    /// <summary> Brushes </summary>
    Brush[] brushes;
    /// <summary> Eraser </summary>
    Eraser eraser;
    /// <summary> Currently active brush </summary>
    int currentBrush;

    /// <summary>
    /// Register actions on object awake, set up brushes
    /// </summary>
    private void Awake()
    {
        // Manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.red, Texture = textures[0], Name = "Brush01" };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = textures[1], Name = "Brush02" };

        // Create eraser
        eraser = new Eraser() { Width = 0.05f, Name = "Eraser" };
            
        // Register actions
        paintRef.action.started += ActivatePaint;
        switchRef.action.started += SwitchBrush;
        eraserRef.action.started += ToggleEraser;

        // Display brush values on canvas
        currentBrush = 0;
        canvas.SwitchBrushValues(brushes[currentBrush]);
    }

    // Update is called once per frame
    void Update()
    {
        // Stop drawing on released button
        if (paintRef.action.WasReleasedThisFrame())
            DisablePaint();

        // Paint
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

        // Display information on canvas
        if (isEraser)
            canvas.SwitchToEraser(eraser);
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
        {
            return;
        }

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
        // Erase
        if (isEraser)
        {
            Collider[] cs = Physics.OverlapSphere(controller.transform.position, eraser.Width / 2.0f);
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i].gameObject.tag == "line")
                    Destroy(cs[i].gameObject);
            }
        }
        // Paint
        else
        {
            paintingOn = true;

            // Instantiate new line
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);

            // TODO add server connection updater thingy

            // Get line renderer
            currLine = o.GetComponent<LineRenderer>();

            // Set color
            currLine.material.SetColor("_Color", brushes[currentBrush].Color);

            // Set texture - only if the brush has any, otherwise stays the default of the material
            if (brushes[currentBrush].Texture != null)
                currLine.material.SetTexture("_MainTex", brushes[currentBrush].Texture);

            // Set line width
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1);
            curve.AddKey(1.0f, 1);
            currLine.widthMultiplier = brushes[currentBrush].Width;
            currLine.widthCurve = curve;

            // Set first 2 positions
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
    /// Change colour of current brush
    /// </summary>
    /// <param name="c"> Colour </param>
    public void ColorChange(Color c)
    {
        if (isEraser)
            return;

        // Set brush color
        if (brushes != null && brushes[currentBrush] != null)
            brushes[currentBrush].Color = c;
    }

    /// <summary>
    /// Set width of current brush
    /// </summary>
    /// <param name="f"> Width </param>
    public void SetBrushWidth(float f)
    {
        // Limit on how small can a brush be
        if (f <= 0.1)
            f = 0.1f;

        // Resize to be smaller
        hand.localScale = new Vector3(0.1f * f, 0.1f * f, 0.1f * f);

        // Set eraser
        if (isEraser)
        {
            eraser.Width = f * 0.1f;
            return;
        }

        // Set brush
        brushes[currentBrush].Width = f * 0.1f;
    }

}
