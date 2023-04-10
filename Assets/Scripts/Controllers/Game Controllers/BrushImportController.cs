using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Script used to import brush data
/// </summary>
public class BrushImportController : MonoBehaviour
{
    [SerializeField]
    PaintController paintController;

    LineImporter li;

    // Start is called before the first frame update
    void Start()
    {
        li = new LineImporter();
        ReloadBrushes();
    }

    /// <summary>
    /// Load brushes from folder Brushes on the same level as the exe
    /// </summary>
    public void ReloadBrushes()
    {
        Debug.Log("Loading brushes...");

        string path = Application.dataPath + "/../Brushes";
        List<Brush> bList = new List<Brush>();

        if (Directory.Exists(path))
        {
            // for all files in folder
            foreach (string fileName in Directory.GetFiles(path))
            {
                // if they are xmls import as brush
                if (fileName.EndsWith(".xml"))
                {
                    Brush b = li.ImportBrush(fileName.Substring(0, fileName.Length - 4));
                    bList.Add(b);
                }
            }
        }

        // add to brushes
        paintController.AddBrushes(bList);
    }
}
