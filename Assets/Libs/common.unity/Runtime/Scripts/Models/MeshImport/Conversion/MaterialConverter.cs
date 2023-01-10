using System;
using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Converter of an Assimp material to Unity.
    /// </summary>
    internal static class MaterialConverter
    {
        private static readonly int SrcBlend = UnityEngine.Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = UnityEngine.Shader.PropertyToID("_DstBlend");
        private static readonly int BumpScale = UnityEngine.Shader.PropertyToID("_BumpScale");
        private static readonly int Color1 = UnityEngine.Shader.PropertyToID("_Color");
        private static readonly int EmissionColor = UnityEngine.Shader.PropertyToID("_EmissionColor");
        private static readonly int GlossyReflections = UnityEngine.Shader.PropertyToID("_GlossyReflections");
        private static readonly int EmissionMap = UnityEngine.Shader.PropertyToID("_EmissionMap");
        private static readonly int ParallaxMap = UnityEngine.Shader.PropertyToID("_ParallaxMap");
        private static readonly int BumpMap = UnityEngine.Shader.PropertyToID("_BumpMap");

        /// <summary>
        /// Converts an Assimp material to Unity and assigns it its textures.
        /// </summary>
        /// <param name="material">The Assimp material.</param>
        /// <param name="unityTextures">All Unity textures in a scene.</param>
        /// <returns></returns>
        public static UnityEngine.Material ConvertToUnity(Assimp.Material material, IReadOnlyList<UnityEngine.Texture> unityTextures)
        {
            var unityMaterial = new UnityEngine.Material(UnityEngine.Shader.Find("Standard"));

            if (material.HasBlendMode)
            {
                SetBlendMode(unityMaterial, material.BlendMode);
            }

            if (material.HasBumpScaling)
            {
                unityMaterial.SetFloat(BumpScale, material.BumpScaling);
            }

            if (material.HasColorAmbient)
            {
            }

            if (material.HasColorDiffuse)
            {
                unityMaterial.SetColor(
                    Color1, 
                    new UnityEngine.Color(
                        material.ColorDiffuse.R, 
                        material.ColorDiffuse.G, 
                        material.ColorDiffuse.B, 
                        material.ColorDiffuse.A));
            }

            if (material.HasColorEmissive)
            {
                unityMaterial.SetColor(
                    EmissionColor, 
                    new UnityEngine.Color(
                        material.ColorEmissive.R, 
                        material.ColorEmissive.G, 
                        material.ColorEmissive.B, 
                        material.ColorEmissive.A));
            }

            if (material.HasColorReflective)
            {
            }

            if (material.HasColorSpecular)
            {
            }

            if (material.HasColorTransparent)
            {
            }

            if (material.HasOpacity)
            {
            }

            if (material.HasReflectivity)
            {
                unityMaterial.SetFloat(GlossyReflections, material.Reflectivity);
            }

            if (material.HasShadingMode)
            {
            }

            if (material.HasShininess)
            {
            }

            if (material.HasShininessStrength)
            {
            }

            if (material.HasTextureAmbient)
            {
            }

            if (material.HasTextureDiffuse)
            {
                // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
                unityMaterial.mainTexture = unityTextures[material.TextureDiffuse.TextureIndex]; 
            }

            if (material.HasTextureDisplacement)
            {
            }

            if (material.HasTextureEmissive)
            {
                unityMaterial.SetTexture(EmissionMap, unityTextures[material.TextureEmissive.TextureIndex]);
            }

            if (material.HasTextureHeight)
            {
                // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
                unityMaterial.SetTexture(ParallaxMap, unityTextures[material.TextureDisplacement.TextureIndex]); 
            }

            if (material.HasTextureLightMap)
            {
            }

            if (material.HasTextureNormal)
            {
                // TODO pravděpodobne špatný způsob jak vyhledat správnou texturu
                unityMaterial.SetTexture(BumpMap, unityTextures[material.TextureNormal.TextureIndex]); 
            }

            if (material.HasTextureOpacity)
            {
            }

            if (material.HasTextureReflection)
            {
            }

            if (material.HasTextureSpecular)
            {
            }

            if (material.HasTwoSided)
            {
            }

            if (material.HasWireFrame)
            {
            }

            return unityMaterial;
        }

        /// <summary>
        /// Sets blending mode to a shader of a material.
        /// </summary>
        /// <param name="material">The Unity material.</param>
        /// <param name="blendMode">Blending mode.</param>
        private static void SetBlendMode(UnityEngine.Material material, Assimp.BlendMode blendMode)
        {
            switch (blendMode)
            {
                case Assimp.BlendMode.Default:
                {
                    material.SetInteger(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInteger(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                    break;
                case Assimp.BlendMode.Additive:
                {
                    material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.One);
                }
                    break;
                default:
                    throw new NotSupportedException($"Blend mode \"{blendMode}\" is not supported.");
            }
        }
    }
}