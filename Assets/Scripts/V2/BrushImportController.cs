using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public void ReloadBrushes()
    {
        string path = Application.dataPath + "/../Brushes";
        List<Brush> bList = new List<Brush>();

        if (Directory.Exists(path))
        {
            Debug.Log(path);

            // for all files in folder
            foreach (string fileName in Directory.GetFiles(path))
            {
                // if they are xmls import as brush
                if (fileName.EndsWith(".xml"))
                {
                    // TODO catch errors
                    Brush b = li.ImportBrush(fileName.Substring(0, fileName.Length - 4));
                    bList.Add(b);
                }

            }
        }

        // add to brushes
        paintController.AddBrushes(bList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
