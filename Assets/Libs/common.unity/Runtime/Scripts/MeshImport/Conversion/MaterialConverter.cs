using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp material to Unity.
    /// </summary>
    internal class MaterialConverter
    {
        /// <summary>
        /// Converts an Assimp material to Unity and assigns it its textures.
        /// </summary>
        /// <param name="material">The Assimp material.</param>
        /// <param name="unityTextures">All Unity textures in a scene.</param>
        /// <returns></returns>
        public UnityEngine.Material ConvertToUnity(Assimp.Material material, List<UnityEngine.Texture> unityTextures)
        {
            var unityMaterial = new UnityEngine.Material(UnityEngine.Shader.Find("Standard"));

            if(material.HasBlendMode)
            {
                this.SetBlendMode(unityMaterial, material.BlendMode);
            }

            if (material.HasBumpScaling)
            {
                unityMaterial.SetFloat("_BumpScale", material.BumpScaling);
            }

            if (material.HasColorAmbient)
            {
                
            }

            if(material.HasColorDiffuse)
            {
                unityMaterial.SetColor("_Color", new UnityEngine.Color(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A));
            }

            if(material.HasColorEmissive)
            {
                unityMaterial.SetColor("_EmissionColor", new UnityEngine.Color(material.ColorEmissive.R, material.ColorEmissive.G, material.ColorEmissive.B, material.ColorEmissive.A));
            }

            if(material.HasColorReflective)
            {

            }

            if(material.HasColorSpecular)
            {

            }

            if(material.HasColorTransparent)
            {

            }

            if(material.HasOpacity)
            {

            }

            if(material.HasReflectivity)
            {
                unityMaterial.SetFloat("_GlossyReflections", material.Reflectivity);
            }

            if(material.HasShadingMode)
            {
                
            }

            if(material.HasShininess)
            {

            }

            if(material.HasShininessStrength)
            {

            }

            if(material.HasTextureAmbient)
            {
                
            }

            if(material.HasTextureDiffuse)
            {
                unityMaterial.mainTexture = unityTextures[material.TextureDiffuse.TextureIndex]; // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
            }

            if(material.HasTextureDisplacement)
            {
                
            }

            if(material.HasTextureEmissive)
            {
                unityMaterial.SetTexture("_EmissionMap", unityTextures[material.TextureEmissive.TextureIndex]);
            }

            if(material.HasTextureHeight)
            {
                unityMaterial.SetTexture("_ParallaxMap", unityTextures[material.TextureDisplacement.TextureIndex]); // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
            }

            if (material.HasTextureLightMap) 
            { 

            }

            if (material.HasTextureNormal) 
            {
                unityMaterial.SetTexture("_BumpMap", unityTextures[material.TextureNormal.TextureIndex]); // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
            }

            if(material.HasTextureOpacity)
            {

            }

            if(material.HasTextureReflection)
            {

            }

            if(material.HasTextureSpecular)
            {

            }

            if(material.HasTwoSided)
            {
                
            }

            if(material.HasWireFrame)
            {

            }

            return unityMaterial;
        }

        /// <summary>
        /// Sets blending mode to a shader of a material.
        /// </summary>
        /// <param name="material">The Unity material.</param>
        /// <param name="blendMode">Blending mode.</param>
        private void SetBlendMode(UnityEngine.Material material, Assimp.BlendMode blendMode)
        {
            switch (blendMode)
            {
                case Assimp.BlendMode.Default:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    }
                    break;
                case Assimp.BlendMode.Additive:
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    }
                    break;
            }
        }
    }
}
