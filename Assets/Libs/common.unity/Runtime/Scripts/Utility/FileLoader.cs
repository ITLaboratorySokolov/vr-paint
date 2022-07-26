// using SimpleFileBrowser;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
// using UnityMeshImporter;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties.Managers;

namespace ZCU.TechnologyLab.Common.Unity.Utility
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
        private UnityEvent<GameObject> OnFileLoaded = new UnityEvent<GameObject> ();

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
        IEnumerator ShowLoadDialogCoroutine()
        {
            throw new NotImplementedException();

            /*
            FileBrowser.SetFilters(false, 
                new FileBrowser.Filter("Obrázek (*.jpg; *.png)", ".jpg", ".png"),
                new FileBrowser.Filter("Mesh (*.obj; *.fbx; *.gltf; *.ply; *.stl)", ".obj", ".fbx", ".gltf", ".ply", ".stl"));

            // Show a load file dialog and wait for a response from user
            // Load file/folder: both, Allow multiple selection: true
            // Initial path: default (Documents), Initial filename: empty
            // Title: "Load File", Submit button text: "Load"
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Otevřít soubory", "Otevřít");

            // Dialog is closed
            // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
            Debug.Log(FileBrowser.Success);

            if (FileBrowser.Success)
            {
                this.ReadFiles(FileBrowser.Result);
            }
            */
        }

        /// <summary>
        /// Reads selected files.
        /// </summary>
        /// <param name="paths">Paths of files.</param>
        private void ReadFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                string extension = Path.GetExtension(path).ToLower();
                GameObject obj;

                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        {
                            obj = ReadImage(path);
                        }
                        break;
                    case ".obj":
                    case ".fbx":
                    case ".gltf":
                    case ".ply":
                    case ".stl":
                        {
                            obj = ReadMesh(path);
                        }
                        break;
                    default:
                        throw new FileLoadException("Unknown file extension.");

                }

                this.OnFileLoaded.Invoke(obj);
            }
        }

        /// <summary>
        /// Creates a bitmap world object from a file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        private GameObject ReadImage(string filePath)
        {
            GameObject obj = new GameObject(Path.GetFileNameWithoutExtension(filePath));
            var data = File.ReadAllBytes(filePath);
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
        private GameObject ReadMesh(string filePath)
        {
            throw new NotImplementedException();

            /*
            GameObject obj = MeshImporter.Load(filePath);

            var mesh = obj.GetComponent<MeshFilter>().mesh;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            obj.AddComponent<MeshPropertiesManager>();

            return obj;
            */
        }
    }
}