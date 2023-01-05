using Assimp;
using System.IO;
using ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion;

namespace ZCU.TechnologyLab.Common.Unity.MeshImport
{
    /// <summary>
    /// The MeshImporter class imports a scene from a 3D file format.
    /// </summary>
    /// <remarks>
    /// Supported file formats are listed here https://github.com/assimp/assimp/blob/master/doc/Fileformats.md.
    /// </remarks>
    public class MeshImporter
    {
        /// <summary>
        /// Convertor of scenes from Assimp to Unity.
        /// </summary>
        private readonly SceneConverter sceneConverter = new SceneConverter();

        /// <summary>
        /// Imports a scene from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Import(Stream stream)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFileFromStream(stream, PostProcessSteps.GlobalScale);
            sceneConverter.ConvertToUnity(scene);
        }

        /// <summary>
        /// Imports a scene from a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void Import(string path)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(path, PostProcessSteps.GlobalScale);
            sceneConverter.ConvertToUnity(scene);
        }
    }
}