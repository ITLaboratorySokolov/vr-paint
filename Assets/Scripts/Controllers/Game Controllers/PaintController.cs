using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables;

/// <summary>
/// Script used to controll the paint actions
/// - reacts to brush swaps and swap between brush and eraser
/// - controls the painting of a line
/// </summary>
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
    RigController rigCont;

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

    /// <summary> Currently painted triangle strip </summary>
    TriangleStrip currentLineStrip;
    /// <summary> Used strip generator </summary>
    TriangleStripGenerator stripGenerator;
    /// <summary> When painting started </summary>
    float startPaintTime;
    /// <summary> Current time </summary>
    float currPaintTime;

    [Header("Brushes")]
    /// <summary> Number of brushes in system </summary>
    [SerializeField]
    int numberOfBrushes = 2;
    /// <summary> Textures for brushes </summary>
    [SerializeField]
    Texture[] textures;
    /// <summary> Brushes </summary>
    internal Brush[] brushes;
    /// <summary> Eraser </summary>
    Eraser eraser;
    /// <summary> Currently active brush </summary>
    int currentBrush;
    Dictionary<string, Brush> brushDictionary;

    [Header("ServerConnection")]
    /// <summary> Server connection controller </summary>
    [SerializeField]
    ServerConectionController serverCont;
    /// <summary> Time until next update </summary>
    float timeToUpdate;
    /// <summary> Is application ready for user input </summary>
    bool inputEnabled;
    /// <summary> Name of client </summary>
    [SerializeField]
    private StringVariable clientName;

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

        Texture2D tx1 = ConvertorHelper.TextureToTexture2D(textures[0]);

        // Manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.red, Texture = tx1, Name = "Brush01", TimePerIter = 5, WidthModifier = yValues };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = null, Name = "Brush02", TimePerIter = 10, WidthModifier = yValues };

        brushDictionary = new Dictionary<string, Brush>();
        brushDictionary.Add(brushes[0].Name, brushes[0]);
        brushDictionary.Add(brushes[1].Name, brushes[1]);

        // Create eraser
        eraser = new Eraser() { Width = 0.05f, Name = "Eraser" };

        // Display brush values on canvas
        currentBrush = 0;
        
        RegisterActions();
        lineCounter = 0;
        stripGenerator = new TriangleStripGenerator();

        paintingEnabled = true;
        timeToUpdate = 0.25f;
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
    }

    /// <summary>
    /// Get first child with given tag
    /// </summary>
    /// <param name="t"> Parent transform </param>
    /// <param name="tag"> Tag </param>
    /// <returns> Child with tag or null </returns>
    private Transform GetChildWithTag(Transform t, string tag)
    {
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
                return tr;

            if (tr.transform.childCount != 0)
            {
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
        // Stop drawing on released painting button
        if (paintRef.action.WasReleasedThisFrame())
            DisablePaint();

        if (!paintingEnabled || !inputEnabled)
            return;

        // Paint if painting button held
        if (paintingOn)
            Paint();
    }

    /// <summary>
    /// Toggle eraser brush
    /// </summary>
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
    private void SwitchBrush(InputAction.CallbackContext obj)
    {
        // If eraser is active or user is currently painting
        if (isEraser || paintingOn)
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
        if (!paintingEnabled || !inputEnabled)
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
                    // TODO funguje to?
                    serverCont.DestroyObjectOnServer(name, cs[i].gameObject);
                    // Destroy(cs[i].gameObject);
                }
            }
        }

        // Paint line
        else
        {
            paintingOn = true;
            startPaintTime = Time.time;

            // create a new mesh
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
            o.name = "Line_" + clientName.Value + "_" + lineCounter;

            float widthModifier = Mathf.Max(0.001f, brushes[currentBrush].WidthModifier[0]);
            float w = brushes[currentBrush].Width * widthModifier;
            currentLineStrip = new TriangleStrip(controllerGrip.position, controllerGrip.up.normalized, w);
            // o.GetComponent<MeshFilter>().mesh = currentLineStrip.mesh;
            
            o.GetComponent<MeshCollider>().enabled = true;
            Debug.Log("Set colliders");

            o.GetComponent<Renderer>().material.SetTexture("_MainTex", brushes[currentBrush].Texture);
            o.GetComponent<Renderer>().material.SetColor("_Color", brushes[currentBrush].Color);

            currLineObj = o;

            serverCont.SendTriangleStripToServer(o, brushes[currentBrush].Texture);
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
        int startIn = 0;
        int endIn = 0;
        float t = 0;

        if (modLen > 1)
        {
            float timestep = b.TimePerIter / (modLen - 1);
            float paintTime = (currPaintTime - startPaintTime) % b.TimePerIter;
            
            startIn = (int)(paintTime / timestep);
            endIn = (startIn + 1) % modLen;
            //if (endIn == modLen)
                //widthModifier = b.WidthModifier[modLen - 1];
            //else
            {
                t = (paintTime % timestep) / timestep; 
                widthModifier = Mathf.Lerp(b.WidthModifier[startIn], b.WidthModifier[endIn], t);
            }
        }

        // Get UVs
        float uSt = 0;
        float uEn = 0;
        if (modLen > 1)
        {
            uSt = startIn / (float)(modLen - 1);
            uEn = endIn / (float)(modLen - 1);
        }
        float u = Mathf.Lerp(uSt, uEn, t);

        // so there is no mesh with points in the same space
        widthModifier = Mathf.Max(0.001f, widthModifier);

        // Generate next part of the triangle strip
        stripGenerator.AddPointToLine(controllerGrip.position, b.Width * widthModifier, controllerGrip.up.normalized, currentLineStrip, u);
        currLineObj.GetComponent<MeshFilter>().mesh = currentLineStrip.mesh;
        currLineObj.GetComponent<MeshCollider>().sharedMesh = currentLineStrip.mesh;

        // Update properties
        timeToUpdate -= Time.deltaTime;
        if (timeToUpdate <= 0.0001)
        {
            timeToUpdate = 0.25f;
            // serverCont.UpdateTriangleStripOnServer(currLineObj);
        }
    }

    /// <summary>
    /// Stop painting
    /// </summary>
    public void DisablePaint()
    {
        if (isEraser || !paintingOn) // || !paintingEnabled 
            return;

        paintingOn = false;
        timeToUpdate = 0.25f;
        serverCont.UpdateTriangleStripOnServer(currLineObj, brushes[currentBrush].Texture);
        lineCounter++;
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
    /// Set width to the width of current brush
    /// </summary>
    /// <param name="f"> Width </param>
    public void SetBrushWidth()
    {
        float f = brushes[currentBrush].Width;
        // Resize
        Vector3 scale = new Vector3(f, f, f);
        rigCont.SetBrushScale(scale);
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

        // Resize
        Vector3 scale = new Vector3(0.1f * f, 0.1f * f, 0.1f * f);
        rigCont.SetBrushScale(scale);

        // Set eraser
        if (isEraser)
        {
            eraser.Width = f * 0.1f;
        }
        // Set brush
        else
        {
            brushes[currentBrush].Width = f * 0.1f;
        }
    }

    /// <summary>
    /// Toggle when this script should react on painting input
    /// Switching between two controller modes, painting and UI interaction
    /// </summary>
    /// <param name="val"> True if painting enabled, false if not </param>
    public void TogglePaintingEnabled(bool val)
    {
        paintingEnabled = val;
    }

    /// <summary>
    /// Add brushes to dictionary of availible brushes
    /// </summary>
    /// <param name="addBrushes"> List of new brushes </param>
    public void AddBrushes(List<Brush> addBrushes)
    {
        Brush[] defaultBrushes = new Brush[2];
        defaultBrushes[0] = brushes[0];
        defaultBrushes[1] = brushes[1];

        brushDictionary = new Dictionary<string, Brush>();
        brushDictionary.Add(brushes[0].Name, brushes[0]);
        brushDictionary.Add(brushes[1].Name, brushes[1]);

        Brush[] newBrushSet = new Brush[defaultBrushes.Length + addBrushes.Count];
        newBrushSet[0] = defaultBrushes[0];
        newBrushSet[1] = defaultBrushes[1];

        for (int i = 0; i < addBrushes.Count; i++)
        {
            newBrushSet[defaultBrushes.Length + i] = addBrushes[i];
            brushDictionary.Add(addBrushes[i].Name, addBrushes[i]);
        }

        brushes = newBrushSet;
        numberOfBrushes = newBrushSet.Length;

        // swap back to first brush
        currentBrush = -1;
        SwitchBrush(new InputAction.CallbackContext());
    }

    /// <summary>
    /// Add line from server
    /// - set its mesh collider
    /// </summary>
    /// <param name="o"> Server line </param>
    internal void AddServerLine(GameObject o)
    {
        // Set collider
        MeshFilter viewedModelFilter = (MeshFilter)o.GetComponent("MeshFilter");
        Mesh mesh = viewedModelFilter.mesh;

        MeshCollider meshCollider = o.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Set if application is ready for user input
    /// </summary>
    /// <param name="value"> True if ready, false if not </param>
    /// <param name="serverLines"> Highest number of line on server </param>
    public void ToggleReadyForInput(bool value, int serverLines = 0)
    {
        inputEnabled = value;
        if (inputEnabled)
        {
            lineCounter = serverLines + 1;
            Debug.Log("Now can start painting! Waiting for Line" + clientName.Value + lineCounter);
        }
        else
            Debug.Log("Painting turned off");
    }


}
