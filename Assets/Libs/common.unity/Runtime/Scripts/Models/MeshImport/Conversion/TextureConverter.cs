namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp texture to Unity.
    /// </summary>
    internal static class TextureConverter
    {
        /// <summary>
        /// Converts an Assimp texture to Unity.
        /// </summary>
        /// <param name="texture">Assimp texture.</param>
        /// <returns>Unity texture.</returns>
        public static UnityEngine.Texture ConvertToUnity(Assimp.EmbeddedTexture texture)
        {
            var unityTexture = new UnityEngine.Texture2D(texture.Width, texture.Height);

            if(texture.IsCompressed && texture.HasCompressedData)
            {
                // Load image from jpg, png and other Unity supported formats
                UnityEngine.ImageConversion.LoadImage(unityTexture, texture.CompressedData);
            } 
            else if (texture.HasNonCompressedData)
            {
                var colors = ConvertTexelToColor(texture);
                unityTexture.SetPixelData(colors, 0);
            }

            // Load texture data to GPU
            unityTexture.Apply();

            return unityTexture;
        }

        private static UnityEngine.Color[] ConvertTexelToColor(Assimp.EmbeddedTexture texture)
        {
            var colors = new UnityEngine.Color[texture.Width * texture.Height];

            for (var i = 0; i < colors.Length; i++)
            {
                var texel = texture.NonCompressedData[i];
                colors[i] = new UnityEngine.Color(texel.R, texel.G, texel.B, texel.A);
            }

            return colors;
        }
    }
}
