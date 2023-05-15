using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using ZCU.TechnologyLab.Common.Entities.DataTransferObjects;

/// <summary>
/// Class for the import of a scene from a folder
/// </summary>
public class SceneImporter
{
    /// <summary>
    /// Import world objects from folder
    /// </summary>
    /// <param name="path"> Path to folder </param>
    /// <returns> List of world objects </returns>
    public List<WorldObjectDto> LoadObjectsFromFolder(string path)
    {
        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;

        List<WorldObjectDto> woList = new List<WorldObjectDto>();

        string[] filePaths = Directory.GetFiles(path);
        foreach (string filePath in filePaths)
        {
            if (!filePath.EndsWith(".txt"))
                continue;

            string[] lines = File.ReadAllLines(filePath);

            string nm = Path.GetFileName(filePath);
            RemoteVectorDto pos = StringToVector(lines[1]);
            RemoteVectorDto rotation = StringToVector(lines[2]);
            RemoteVectorDto scale = StringToVector(lines[3]);

            Dictionary<string, byte[]> props = new Dictionary<string, byte[]>();
            for (int i = 4; i < lines.Length; i+=2)
            {
                string name = lines[i].Trim();
                byte[] bytes = Convert.FromBase64String(lines[i+1]);
                props.Add(name, bytes);
            }

            WorldObjectDto o = new WorldObjectDto();
            o.Name = nm.Substring(0, nm.Length - 4);
            o.Type = lines[0].Trim();
            o.Position = pos;
            o.Rotation = rotation;
            o.Scale = scale;
            o.Properties = props;

            woList.Add(o);
        }

        return woList;
    }

    /// <summary>
    /// Parse a string into RemoteVectorDto
    /// </summary>
    /// <param name="line"> Line to parse </param>
    /// <returns> Parsed vector </returns>
    private RemoteVectorDto StringToVector(string line)
    {
        string[] vecS = line.Split(",");

        if (vecS.Length < 3)
            return new RemoteVectorDto();

        try
        {
            RemoteVectorDto vec = new RemoteVectorDto();
            vec.X = (float)Double.Parse(vecS[0].Trim());
            vec.Y = (float)Double.Parse(vecS[1].Trim());
            vec.Z = (float)Double.Parse(vecS[2].Trim());
            return vec;
        }
        catch
        {
            return new RemoteVectorDto();
        }
    }

}
