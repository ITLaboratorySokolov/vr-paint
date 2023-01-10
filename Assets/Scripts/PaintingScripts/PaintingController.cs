using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Serialization.Mesh;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

// TODO - updates during painting

/// <summary> Class controlling painting </summary>
public class PaintingController : MonoBehaviour
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
    [SerializeField]
    public ObjectController objController;
    /// <summary> Color palette menu canvas controller </summary>
    [SerializeField]
    MenuCanvasController canvas;
    /// <summary> Painting hand controller grip </summary>
    [SerializeField]
    Transform controller;
    /// <summary> Is user currently painting </summary>
    bool paintingOn = false;
    /// <summary> Painting hand object </summary>
    [SerializeField]
    Transform hand;

    [Header("Controlls")]
    /// <summary> Movement speed </summary>
    public int speed;
    /// <summary> Is currently eraser enabled </summary>
    bool isEraser;

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
    /// <summary> Texture controller </summary>
    [SerializeField]
    TextureController txCont;


    [Header("Connection")]
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConnection server;
    /// <summary> Line counter - number of present lines </summary>
    int lineCounter;
    /// <summary> Mesh serializer </summary>
    RawMeshSerializer serializer;
    /// <summary> Is application ready for painting  </summary>
    bool readyForPaint;

    float timeToUpdate = 0.25f;

    /// <summary>
    /// Register actions on object awake, set up brushes
    /// </summary>
    private void Awake()
    {
        readyForPaint = false;
        serializer = new RawMeshSerializer();

        // Manually create two brushes
        brushes = new Brush[numberOfBrushes];
        brushes[0] = new Brush() { Width = 0.05f, Color = Color.red, Texture = textures[0], Name = "Brush01" };
        brushes[1] = new Brush() { Width = 0.05f, Color = Color.black, Texture = textures[1], Name = "Brush02" };

        // Create eraser
        eraser = new Eraser() { Width = 0.05f, Name = "Eraser" };

        // Display brush values on canvas
        currentBrush = 0;
        canvas.SwitchBrushValues(brushes[currentBrush]);

        RegisterActions();
        lineCounter = 0;

        /*
        Debug.Log("Create Line - own curve");
        GameObject o = Instantiate(simpleLine);
        LineRenderer lr3 = o.GetComponent<LineRenderer>();
        AnimationCurve a = new AnimationCurve();
        a.AddKey(0, 0);
        a.AddKey(0.5f, 1);
        a.AddKey(1, 0);
        lr3.widthCurve = a;
        WriteKeys(lr3);
        */
    }

    public void RegisterActions()
    {
        // Register actions
        paintRef.action.started += ActivatePaint;
        switchRef.action.started += SwitchBrush;
        eraserRef.action.started += ToggleEraser;
    }

    private void WriteKeys(LineRenderer lr)
    {
        AnimationCurve c = lr.widthCurve;
        Keyframe[] ks = c.keys;
        Debug.Log(ks.Length);
        for (int i = 0; i < ks.Length; i++)
            Debug.Log(ks[i].time + " " + ks[i].value);

    }

    /*
    /// <summary>
    /// Insert a line from server into the scene
    /// </summary>
    /// <param name="obj"> Server line </param>
    internal void AddServerLine(WorldObjectDto obj)
    {
        // Instantiate new line
        GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);

        // Update properties
        MeshPropertiesManager propsManager = o.GetComponent<MeshPropertiesManager>();
        propsManager.name = obj.Name;
        o.name = obj.Name;

        // Deseralize properties from obj
        var indices = serializer.IndicesSerializer.Deserialize(obj.Properties);
        var vertices = serializer.VerticesSerializer.Deserialize(obj.Properties);
        Vector3[] vectors = ConvertorHelper.FloatsToVec3(vertices);

        // Not a valid line
        if (vertices.Length < 9)
            return;

        // TODO Set collider
        Mesh mesh = new Mesh();
        mesh.SetVertices(vectors);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        MeshCollider meshCollider = o.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        propsManager.SetProperties(obj.Properties);
    }
    */

    internal void AddServerLine(GameObject o)
    {
        o.transform.SetParent(lineParent);
        
        // Set collider
        MeshFilter viewedModelFilter = (MeshFilter)o.GetComponent("MeshFilter");
        Mesh mesh = viewedModelFilter.mesh;

        MeshCollider meshCollider = o.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Update called once per frame
    /// </summary>
    void Update()
    {
        // If application is ready for paint
        if (!readyForPaint)
        {
            if (server.syncCallDone)
            {
                lineCounter = server.serverLines + 1;
                readyForPaint = true;
                Debug.Log("Now can start painting! Waiting for Line" + lineCounter);
            }
            else
                return;
        }

        // Stop drawing on released button
        if (paintRef.action.WasReleasedThisFrame())
            DisablePaint();

        // Paint
        if (paintingOn)
            Paint();
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
        // Erase
        if (isEraser)
        {
            Collider[] cs = Physics.OverlapSphere(controller.transform.position, eraser.Width / 2.0f);
            for (int i = 0; i < cs.Length; i++)
            {
                string name = cs[i].gameObject.name;
                if (cs[i].gameObject.tag == "line")
                {
                    objController.DestroyObject(name, cs[i].gameObject);
                }
            }
        }

        // Paint
        else
        {
            paintingOn = true;
            txCont.StartGenerating();

            // Instantiate new line
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
            o.name = "Line" + lineCounter;

            // Get line renderer
            currLine = o.GetComponent<LineRenderer>();
            currLineObj = o;

            // Set color
            currLine.material.SetColor("_Color", brushes[currentBrush].Color);

            // Set texture - only if the brush has any, otherwise stays the default of the material
            // TODO setting texture from txCont
            // if (brushes[currentBrush].Texture != null)
            //    currLine.material.SetTexture("_MainTex", brushes[currentBrush].Texture);
            currLine.material.SetTexture("_MainTex", txCont.generatedTexture);


            // Set line width

            Keyframe[] keys = new Keyframe[3];
            keys[0] = new Keyframe(0.1f, 0.2f);
            keys[1] = new Keyframe(0.5f, 1);
            keys[2] = new Keyframe(0.9f, 0.2f);
            AnimationCurve curve = new AnimationCurve(keys);
            currLine.widthMultiplier = brushes[currentBrush].Width;
            currLine.widthCurve = curve;

            // TODO tady to je všechno 2 - proè???
            Debug.Log("New line created");
            WriteKeys(currLine);

            // Set first 2 positions
            currLine.SetPosition(0, controller.position);
            currLine.SetPosition(1, controller.position);

            // Send to server
            Mesh mesh = new Mesh();
            currLine.BakeMesh(mesh);

            // Set properties
            MeshPropertiesManager propsManager = currLineObj.GetComponent<MeshPropertiesManager>();
            propsManager.name = currLineObj.name;
            Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");
            propsManager.SetProperties(props);
            objController.AddObjectAsync(currLineObj);
        }
    }

    /// <summary>
    /// Painting, adding another position to line
    /// </summary>
    private void Paint()
    {
        currLine.positionCount++;
        currLine.SetPosition(currLine.positionCount - 1, controller.position);
        currLine.material.SetTexture("_MainTex", txCont.generatedTexture);

        // Update properties
        timeToUpdate -= Time.deltaTime;
        if (timeToUpdate <= 0.0001)
        {
            timeToUpdate = 0.25f;

            MeshPropertiesManager propsManager = currLineObj.GetComponent<MeshPropertiesManager>();
            Mesh mesh = new Mesh();
            currLine.BakeMesh(mesh);

            Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");
            propsManager.SetProperties(props);

            // Send update to server
            objController.UpdateProperties(propsManager.name);
        }
    }

    /// <summary>
    /// Stop painting
    /// </summary>
    private void DisablePaint()
    {
        if (isEraser)
            return;

        txCont.StopGenerating();
        paintingOn = false;
        Vector3[] points = new Vector3[currLine.positionCount];

        timeToUpdate = 0.25f;
        MeshPropertiesManager propsManager = currLineObj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = new Mesh();
        currLine.BakeMesh(mesh);

        // TODO Set collider
        MeshCollider meshCollider = currLine.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        Dictionary<string, byte[]> props = serializer.Serialize(ConvertorHelper.Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");
        propsManager.SetProperties(props);

        // Send last update to server
        objController.UpdateProperties(propsManager.name);
        lineCounter++;
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
