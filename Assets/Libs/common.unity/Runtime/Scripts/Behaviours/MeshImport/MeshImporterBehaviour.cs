using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using ZCU.TechnologyLab.Common.Unity.Models.MeshImport;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.MeshImport
{
    /// <summary>
    /// Behaviour wrapper of the <see cref="MeshImporter"/> class. 
    /// Which allows the importer to be placed in a scene and import a mesh on a start.
    /// </summary>
    /// <seealso cref="MeshImporter"/>
    public class MeshImporterBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Serialized path of a 3D file assigned from Unity inspector.
        /// </summary>
        [Tooltip("Path of a 3D file.")]
        [SerializeField]
        [FormerlySerializedAs("path")]
        private string _path;

        /// <summary>
        /// Import the mesh on start of a scene.
        /// </summary>
        [Tooltip("Import the mesh on start of a scene.")]
        [SerializeField]
        [FormerlySerializedAs("importOnStart")]
        private bool _importOnStart;

        /// <summary>
        /// Imports a 3D file on start of a scene when the <see cref="_importOnStart"/> is true.
        /// The path to the 3D file is specified in the <see cref="_path"/> field.
        /// </summary>
        public void Start()
        {
            if(_importOnStart)
            {
                Import();
            }
        }

        /// <summary>
        /// Imports a 3D file when the <see cref="_path"/> is specified in the Unity inspector.
        /// Otherwise it throws an exception.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when path is not specified.</exception>
        public void Import()
        {
            if(string.IsNullOrEmpty(_path))
            {
                throw new ArgumentException("Path is not specified in the inspector.");
            }

            MeshImporter.Import(_path);
        }

        /// <summary>
        /// Imports a 3D file from a path.
        /// </summary>
        /// <param name="path">The path of a file.</param>
        public void Import(string path)
        {
            MeshImporter.Import(path);
        }

        /// <summary>
        /// Imports a 3D scene from a stream.
        /// Stream should contain one of 3D formats.
        /// </summary>
        /// <param name="stream">The stream with 3D format.</param>
        public void Import(Stream stream)
        {
            MeshImporter.Import(stream);
        }
    }
}