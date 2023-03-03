//using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.Utility
{
    /// <summary>
    /// Loads files from file system with a file dialog.
    /// 
    /// When files are selected an object with corresponding world object type is created.
    /// Created object is reported afterwards by an event. The world object is not added to world object manager.
    /// If you need to add it, do it in a method that you assign to the event.
    /// </summary>
    public class FileLoader : MonoBehaviour
    {
        /// <summary>
        /// Event called when a file is loaded and game object is created according to a type of a file.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("OnFileLoaded")]
        private UnityEvent<GameObject> _onFileLoaded = new();

        /// <summary>
        /// Opens a file dialog.
        /// </summary>
        public void OpenLocalFile()
        {
            StartCoroutine(ShowLoadDialogCoroutine());
        }

        /// <summary>
        /// Opens a file dialog and waits for user input. Loaded file is then parsed and object is added to an active world space.
        /// </summary>
        /// <returns>An enumerator.</returns>
        private IEnumerator ShowLoadDialogCoroutine()
        {
            throw new NotImplementedException();

            /*
            FileBrowser.SetFilters(
                false, 
                new FileBrowser.Filter("Obrázek (*.jpg; *.png)", ".jpg", ".png"),
                new FileBrowser.Filter("Mesh (*.obj; *.fbx; *.gltf; *.ply; *.stl)", ".obj", ".fbx", ".gltf", ".ply", ".stl"));

            // Show a load file dialog and wait for a response from user
            // Load file/folder: both, Allow multiple selection: true
            // Initial path: default (Documents), Initial filename: empty
            // Title: "Load File", Submit button text: "Load"
            yield return FileBrowser.WaitForLoadDialog(
                FileBrowser.PickMode.Files, 
                true, 
                null, 
                null, 
                "Otevřít soubory", 
                "Otevřít");

            // Dialog is closed
            // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
            Debug.Log(FileBrowser.Success);

            if (FileBrowser.Success)
            {
                ReadFiles(FileBrowser.Result);
            }
            */
        }

        /// <summary>
        /// Reads selected files.
        /// </summary>
        /// <param name="paths">Paths of files.</param>
        private void ReadFiles(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var extension = Path.GetExtension(path).ToLower();
                var obj = extension switch
                {
                    ".jpg" or ".png" => ReadImage(path),
                    ".obj" or ".fbx" or ".gltf" or ".ply" or ".stl" => ReadMesh(path),
                    _ => throw new FileLoadException("Unknown file extension.")
                };

                _onFileLoaded.Invoke(obj);
            }
        }

        /// <summary>
        /// Creates a bitmap world object from a file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        private static GameObject ReadImage(string path)
        {
            var obj = new GameObject(Path.GetFileNameWithoutExtension(path));
            var data = File.ReadAllBytes(path);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(data);
            texture.Apply();
            
            var propertiesManager = obj.AddComponent<BitmapPropertiesManager>();
            propertiesManager.SetTexture(texture);

            return obj;
        }

        /// <summary>
        /// Creates a mesh world object from a file.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        /// <returns></returns>
        private static GameObject ReadMesh(string filePath)
        {
            // GameObject obj = MeshImporter.Load(filePath);
            //
            // var mesh = obj.GetComponent<MeshFilter>().mesh;
            // mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            //
            // obj.AddComponent<MeshPropertiesManager>();
            //
            // return obj;

            return new GameObject(); // TODO FIX
        }
    }
}