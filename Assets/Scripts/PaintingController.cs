using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;
using ZCU.TechnologyLab.Common.Serialization;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties;

// TODO - what if you switch to eraser during painting
// TODO - doesnt allow me to add custom properties - why and how do i circumvent this

/// <summary> Class controlling painting </summary>
public class PaintingController : MonoBehaviour
{
    [Header("Action input references")]
    /// <summary> Painting action </summary>
    [SerializeField]
    InputActionReference paintRef = null;
    /// <summary> Switch brush action </summary>
    [SerializeField]
    InputActionReference switchRef = null;
    /// <summary> Switch to eraser action </summary>
    [SerializeField]
    InputActionReference eraserRef = null;

    [Header("Game objects")]
    [SerializeField]
    ObjectController objController;
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


    [Header("Connection")]
    /// <summary> Server connection </summary>
    [SerializeField]
    ServerConnection server;
    /// <summary> Line counter - number of present lines </summary>
    int lineCounter;
    /// <summary> Mesh serializer </summary>
    MeshSerializer serializer;
    /// <summary> Is application ready for painting  </summary>
    bool readyForPaint;

    /// <summary>
    /// Register actions on object awake, set up brushes
    /// </summary>
    private void Awake()
    {
        readyForPaint = false;
        serializer = new MeshSerializer();

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

        lineCounter = 0;
    }

    /// <summary>
    /// Insert a line from server into the scene
    /// </summary>
    /// <param name="obj"> Server line </param>
    internal void AddServerLine(WorldObjectDto obj)
    {
        // Instantiate new line
        GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);

        // Update properties
        MeshPropertiesManager propsManager2 = o.GetComponent<MeshPropertiesManager>();
        propsManager2.name = obj.Name;
        o.name = obj.Name;

        // Deseralize properties from obj
        var indices = serializer.DeserializeIndices(obj.Properties);
        var vertices = serializer.DeserializeVertices(obj.Properties);
        Vector3[] vectors = new Vector3[vertices.Length / 3];

        // Not a valid line
        if (vertices.Length < 9)
            return;

        int index = 0;
        for (int i = 0; i < vertices.Length; i += 3)
        {
            vectors[index] = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
            index++;
        }
        propsManager2.SetProperties(obj.Properties);

        // Update line properties - vertices and indices
        LineRenderer l = o.GetComponent<LineRenderer>();
        MeshCollider meshCollider = l.GetComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        mesh.SetVertices(vectors);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        // Set color - default is red

        // TODO Texture
        // TODO Color

        var mat = o.GetComponent<MeshRenderer>().material;
        mat.SetColor("_Color", Color.red);
        if (obj.Properties.ContainsKey("Color"))
        {
            byte[] bytes = obj.Properties["Color"];
            string str = Encoding.ASCII.GetString(bytes);
            string[] vals = str.Split(",");

            int r = 0; int g = 0; int b = 0;
            int.TryParse(vals[0].Trim(), out r);
            int.TryParse(vals[1].Trim(), out g);
            int.TryParse(vals[2].Trim(), out b);

            Color c = new Color(r, g, b);
            mat.SetColor("_Color", c);
            Debug.Log(str);
        }

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
    /// Array of Vector3 to array of floats
    /// </summary>
    /// <param name="vecs"> Array of Vector3 </param>
    /// <returns></returns>
    public float[] Vec3ToFloats(Vector3[] vecs)
    {
        float[] floats = new float[vecs.Length * 3];

        //Convert each vector to floats
        int index = 0;
        for (int i = 0; i < vecs.Length; i++)
        {
            floats[index] = vecs[i].x;
            floats[index+1] = vecs[i].y;
            floats[index+2] = vecs[i].z;
            index += 3;
        }

        return floats;
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
                {
                    objController.DestroyObject(gameObject.name);
                    Destroy(cs[i].gameObject);
                }
            }
        }
        // Paint
        else
        {
            paintingOn = true;

            // Instantiate new line
            GameObject o = Instantiate(simpleLine, lineParent.position, lineParent.rotation, lineParent);
            o.name = "Line" + lineCounter;

            // Get line renderer
            currLine = o.GetComponent<LineRenderer>();
            currLineObj = o;

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

            /* TODO send "empty" line to server
            // Send to server
            MeshPropertiesManager propsManager = o.GetComponent<MeshPropertiesManager>();
            propsManager.name = o.name;
            Mesh mesh = new Mesh();
            currLine.BakeMesh(mesh);
            Dictionary<string, byte[]> props = serializer.SerializeProperties(Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "triangle");
            propsManager.SetProperties(props);
            lineCounter++;

            objController.AddObjectAsync(o);
            */
        }
    }

    /// <summary>
    /// Painting, adding another position to line
    /// </summary>
    private void Paint()
    {
        currLine.positionCount++;
        currLine.SetPosition(currLine.positionCount - 1, controller.position);

        /* TODO update server line
        // Update properties
        MeshPropertiesManager propsManager = currLineObj.GetComponent<MeshPropertiesManager>();
        Mesh mesh = new Mesh();
        currLine.BakeMesh(mesh);

        Dictionary<string, byte[]> props = serializer.SerializeProperties(Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "triangle");
        
        Debug.Log(mesh.GetIndices(0).Length);
        Debug.Log(Vec3ToFloats(mesh.vertices).Length);

        propsManager.SetProperties(props);

        // Send update to server
        objController.UpdateProperties(propsManager.name);
        */
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
        currLine.BakeMesh(mesh);

        // TODO Send to server
        MeshPropertiesManager propsManager = currLineObj.GetComponent<MeshPropertiesManager>();
        propsManager.name = currLineObj.name;
        Dictionary<string, byte[]> props = serializer.SerializeProperties(Vec3ToFloats(mesh.vertices), mesh.GetIndices(0), "Triangle");

        // Add color to properties
        Color currC = currLine.material.GetColor("_Color");
        string currCval = currC.r + ", " + currC.g + ", " + currC.b;
        byte[] bytes = Encoding.ASCII.GetBytes(currCval);
        props.Add("Color", bytes);

        propsManager.SetProperties(props);
        lineCounter++;

        objController.AddObjectAsync(currLineObj);

        var mat = currLineObj.GetComponent<MeshRenderer>().material;
        mat.SetColor("_Color", brushes[currentBrush].Color);
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
