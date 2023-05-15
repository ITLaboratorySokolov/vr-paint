using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

/// <summary>
/// Class for the exports of a scene to a folder
/// </summary>
public class SceneExporter
{

    /// <summary>
    /// Save server objects to folder
    /// </summary>
    /// <param name="path"> Path to folder </param>
    /// <param name="gameobj"> List of objects to save </param>
    public void SaveObjectsToFolder(string path, List<GameObject> gameobj)
    {
        // clear data from folder
        string[] filePaths = Directory.GetFiles(path);
        foreach (string filePath in filePaths)
            File.Delete(filePath);

        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;

        // save all objects to files
        foreach (GameObject o in gameobj)
        {
            MeshPropertiesManager mpm = o.GetComponent<MeshPropertiesManager>();
            BitmapPropertiesManager bpm = o.GetComponent<BitmapPropertiesManager>();

            string data = "";
            Dictionary<string, byte[]> props = null;
            if (mpm != null)
            {
                data += "mesh\n";
                props = mpm.GetProperties();
            }
            else if (bpm != null)
            {
                data += "bitmap\n";
                props = bpm.GetProperties();
            }

            data += $"{o.transform.position.x}, {o.transform.position.y}, {o.transform.position.z}\n";
            data += $"{o.transform.eulerAngles.x}, {o.transform.eulerAngles.y}, {o.transform.eulerAngles.z}\n";
            data += $"{o.transform.localScale.x}, {o.transform.localScale.y}, {o.transform.localScale.z}\n";

            var keys = props.Keys;
            foreach (string key in keys)
            {
                Debug.Log(key);
                byte[] p = props[key];
                data += key + "\n";
                data += Convert.ToBase64String(p);
                data += "\n";
            }

            // save to file with name of object
            File.WriteAllText(path + o.name + ".txt", data);
        }
    }

}
