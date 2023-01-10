using Assimp;
using System.IO;
using ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport
{
    /// <summary>
    /// The MeshImporter class imports a scene from a 3D file format.
    /// </summary>
    /// <remarks>
    /// Supported file formats are listed here https://github.com/assimp/assimp/blob/master/doc/Fileformats.md.
    /// </remarks>
    public static class MeshImporter
    {
        /// <summary>
        /// Imports a scene from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static void Import(Stream stream)
        {
            var importer = new AssimpContext();
            var scene = importer.ImportFileFromStream(stream, PostProcessSteps.GlobalScale);
            SceneConverter.ConvertToUnity(scene);
        }

        /// <summary>
        /// Imports a scene from a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public static void Import(string path)
        {
            var importer = new AssimpContext();
            var scene = importer.ImportFile(path, PostProcessSteps.GlobalScale);
            SceneConverter.ConvertToUnity(scene);
        }
    }
}