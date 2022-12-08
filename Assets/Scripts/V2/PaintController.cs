using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaintController : MonoBehaviour
{
    [Header("Action input references")]
    /// <summary> Painting action </summary>
    [SerializeField]
    internal InputActionReference paintRef = null;
    /// <summary> Switch brush action </summary>
    [SerializeField]
    internal InputActionReference switchRef = null;
    /// <summary> Switch to eraser action </summary>
    [SerializeField]
    internal InputActionReference eraserRef = null;

    [Header("Game objects")]
    /// <summary> Color palette menu canvas controller </summary>
    [SerializeField]
    MenuController canvas;
    /// <summary> Painting hand controller grip </summary>
    [SerializeField]
    Transform controllerGrip;
    /// <summary> Painting hand object </summary>
    [SerializeField]
    Transform handObj;

    [Header("Controlls")]
    /// <summary> Is user currently painting </summary>
    bool paintingOn = false;
    /// <summary> Is currently eraser enabled </summary>
    bool isEraser;
    /// <summary> Is PaintController script enabled right now </summary>
    bool paintingEnabled;

    [Header("Line Rendering")]
    /// <summary> Line prefab </summary>
    [SerializeField]
    GameObject simpleLine;
    /// <summary> Line parent </summary>
    [SerializeField]
    Transform lineParent;
    /// <summary> Current line </summary>
    internal LineRenderer currLine;
    /// <summary> Current line game object </summary>
    internal GameObject currLineObj;
    /// <summary> Line counter - number of present lines </summary>
    internal int lineCounter;

    TriangleStrip currentLineStrip;
    TriangleStripGenerator stripGenerator;
    float startPaintTime;
    float currPaintTime;

    [Header("Brushes")]
    /// <summary> Number of brushes in system </summary>
    [SerializeField]
    int numberOfBrushes = 2;
    /// <summary> Textures for brushes </summary>
    [SerializeField]
    Texture2D[] textures;
    /// <summary> Brushes </summary>
    Brush[] brushes;
    /// <summary> Eraser </summary>
    Eraser eraser;
    /// <summary> Currently active brush </summary>
    int currentBrush;

    /// <summary>
    /// Awake - called once before Start
    /// </summary>
    private void Awake()
    {
        // Create width modifiers
        double xStep = 1.0 / 5.0;
        float[] yValues = new float[5 + 1];
        for (int i = 0; i < 5 + 1; i++)
        {
            double  xValue = i * xStep;
            yValues[i] = (Mathf.Sin((float) (xValue * 2 * Mathf.PI)) + 1) / 2.0f;
        }

        // Manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.red, Texture = textures[0], Name = "Brush01", TimePerIter = 5, WidthModifier = yValues };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = textures[1], Name = "Brush02", TimePerIter = 10, WidthModifier = yValues };

        // Create eraser
        eraser = new Eraser() { Width = 0.05f, Name = "Eraser" };

        // Display brush values on canvas
        currentBrush = 0;
        
        RegisterActions();
        lineCounter = 0;
        stripGenerator = new TriangleStripGenerator();

        paintingEnabled = true;
    }

    /// <summary>
    /// Register actions to input action references
    /// </summary>
    public void RegisterActions()
    {
        // Register actions
        paintRef.action.started += ActivatePaintButton;
        switchRef.action.started += SwitchBrush;
        eraserRef.action.started += ToggleEraser;
    }

    /// <summary>
    /// OnDestroy - called upon destroying the object
    /// Unregister actions
    /// </summary>
    private void OnDestroy()
    {
        paintRef.action.started -= ActivatePaintButton;
        switchRef.action.started -= SwitchBrush;
        eraserRef.action.started -= ToggleEraser;
    }

    private void Start()
    {
        canvas.SwitchBrushValues(brushes[currentBrush]);

        // TODO late start 2s late, but i do not like this. what about computer speed
        // how can i better reach hand object
        StartCoroutine(LateStart(2));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Your Function You Want to Call
        Transform t = GetChildWithTag(controllerGrip.transform, "hand");
        handObj = t;
        Debug.Log(t.name);
    }

    private Transform GetChildWithTag(Transform t, string tag)
    {
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
                return tr;

            if (tr.transform.childCount != 0) {
                Transform ct = GetChildWithTag(tr, tag);
                if (ct != null)
                    return ct;
            }
        }

        return null;
    }

    /// <summary>
    /// Update - called once per frame
    /// </summary>
    void Update()
    {
        if (!paintingEnabled)
            return;

        // Stop drawing on released painting button
        if (paintRef.action.WasReleasedThisFrame())
            DisablePaint();

        // Paint if painting button held
        if (paintingOn)
            Paint();
    }

    /// <summary>
    /// Toggle eraser brush
    /// </summary>
    /// <param name="obj"></param>
    private void ToggleEraser(InputAction.CallbackContext obj)
    {
        // If currently painting
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
        // If eraser is active
        if (isEraser || !paintingEnabled)
            return;

        currentBrush++;
        currentBrush %= numberOfBrushes;
        canvas.SwitchBrushValues(brushes[currentBrush]);
    }

    /// <summary>
    /// Activate paint button, has two modes:
    /// Start painting
    /// - spawn new line and set values according to brush
    /// Erase
    /// - delete all line objects the painting hand collides with
    /// </summary>
    /// <param name="ctx"> Callback context </param>
    private void ActivatePaintButton(InputAction.CallbackContext ctx)
    {
        if (!paintingEnabled)
            return;

        // Erase objects
        if (isEraser)
        {
            Collider[] cs = Physics.OverlapSphere(controllerGrip.transform.position, eraser.Width / 2.0f);
            for (int i = 0; i < cs.Length; i++)
            {
                string name = cs[i].gameObject.name;
                if (cs[i].gameObject.tag == "line")
                {
                    Destroy(cs[i].gameObject);
                }
            }
        }

        // Paint line
        else
        {
            paintingOn = true;
            startPaintTime = Time.time;

            // TODO create a new mesh
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
            currentLineStrip = new TriangleStrip(controllerGrip.position);
            o.GetComponent<MeshFilter>().mesh = currentLineStrip.mesh;
            currLineObj = o;
        }
    }

    /// <summary>
    /// Painting, adding another position to line
    /// </summary>
    private void Paint()
    {
        // Get width
        currPaintTime = Time.time;
        Brush b = brushes[currentBrush];
        int modLen = b.WidthModifier.Length;
        float widthModifier = b.WidthModifier[0];
        
        if (modLen != 1)
        {
            float timestep = b.TimePerIter / (modLen - 1);
            float paintTime = (currPaintTime - startPaintTime) % b.TimePerIter;
            
            int startIn = (int)(paintTime / timestep);
            int endIn = startIn + 1;
            if (endIn == modLen)
                widthModifier = b.WidthModifier[modLen - 1];
            else
            {
                float t = paintTime - (timestep * startIn);
                widthModifier = Mathf.Lerp(b.WidthModifier[startIn], b.WidthModifier[endIn], t);
            }
        }

        // Generate next part of the triangle strip
        stripGenerator.AddPointToLine(controllerGrip.position, b.Width * widthModifier, controllerGrip.up.normalized, currentLineStrip);
        currLineObj.GetComponent<MeshFilter>().mesh = currentLineStrip.mesh;
        currLineObj.GetComponent<MeshCollider>().sharedMesh = currentLineStrip.mesh;

        // Update properties

    }

    /// <summary>
    /// Stop painting
    /// </summary>
    private void DisablePaint()
    {
        if (isEraser || !paintingEnabled)
            return;

        paintingOn = false;
    }

    // -------------------------------------------------------------------------------------------------------------------------

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
        Debug.Log("Setting brush width " + f);

        // Limit on how small can a brush be
        if (f <= 0.1)
            f = 0.1f;

        // Resize to be smaller
        if (handObj != null)
            handObj.localScale = new Vector3(0.1f * f, 0.1f * f, 0.1f * f);

        // Set eraser
        if (isEraser)
        {
            eraser.Width = f * 0.1f;
            return;
        }

        // Set brush
        brushes[currentBrush].Width = f * 0.1f;
    }

    /// <summary>
    /// Toggle when this script should react on painting input
    /// Switching between two controller modes, painting and UI interaction
    /// </summary>
    /// <param name="val"></param>
    public void TogglePaintingEnabled(bool val)
    {
        paintingEnabled = val;
    }

}
