namespace ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp texture to Unity.
    /// </summary>
    internal class TextureConverter
    {
        /// <summary>
        /// Converts an Assimp texture to Unity.
        /// </summary>
        /// <param name="texture">Assimp texture.</param>
        /// <returns>Unity texture.</returns>
        public UnityEngine.Texture ConvertToUnity(Assimp.EmbeddedTexture texture)
        {
            var unityTexture = new UnityEngine.Texture2D(texture.Width, texture.Height);

            if(texture.IsCompressed && texture.HasCompressedData)
            {
                // Load image from jpg, png and other Unity supported formats
                UnityEngine.ImageConversion.LoadImage(unityTexture, texture.CompressedData);
            } 
            else if (texture.HasNonCompressedData)
            {
                // Convert array of Assimp.Texel to array of UnityEngine.Color
                UnityEngine.Color[] colors = new UnityEngine.Color[texture.Width * texture.Height];

                for(int i = 0; i < colors.Length; i++)
                {
                    Assimp.Texel texel = texture.NonCompressedData[i];
                    colors[i] = new UnityEngine.Color(texel.R, texel.G, texel.B, texel.A);
                }

                unityTexture.SetPixelData(colors, 0);
            }

            // Load texture data to GPU
            unityTexture.Apply();

            return unityTexture;
        }
    }
}
