using System;
using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Convertor of an Assimp animation to Unity.
    /// </summary>
    internal static class AnimationConverter
    {
        /// <summary>
        /// Converts Assimp animation to Unity. 
        /// Animation is converted to an animation clip, which is assigned to a game object via its name.
        /// Because of it the method binds name of the game object (node) to the animation clip.
        /// </summary>
        /// <param name="animation">Assimp animation.</param>
        /// <returns>Binding between name of a game object and an animation.</returns>
        public static NodeClipBinding ConvertToUnity(Assimp.Animation animation)
        {
            var clipBinding = new NodeClipBinding();

            // Conversion of a mesh animation (animation of vertices)
            if (animation.HasMeshAnimations)
            {
                for(int i = 0; i < animation.MeshAnimationChannelCount; i++)
                {
                    // TODO: animation of mesh vertices
                    //animation.MeshAnimationChannels[i].
                }
            }

            if (animation.HasNodeAnimations)
            {
                ConvertKeyframeAnimation(animation, clipBinding);
            }

            return clipBinding;
        }

        private static void ConvertKeyframeAnimation(Assimp.Animation animation, NodeClipBinding clipBinding)
        {
            for (int i = 0; i < animation.NodeAnimationChannelCount; i++)
            {
                var animationChannel = animation.NodeAnimationChannels[i];
                var clip = new UnityEngine.AnimationClip();

                // Set position keys
                if (animationChannel.HasPositionKeys)
                {
                    ConvertVectorCurves(animationChannel.PositionKeys, clip, "localPosition",
                        ConvertVectorKeyToKeyframes);
                }

                // Set rotation keys
                if (animationChannel.HasRotationKeys)
                {
                    ConvertVectorCurves(animationChannel.RotationKeys, clip, "localRotation",
                        ConvertQuaternionKeyToKeyframes);
                }

                // Set scaling keys
                if (animationChannel.HasScalingKeys)
                {
                    ConvertVectorCurves(animationChannel.ScalingKeys, clip, "scale", ConvertVectorKeyToKeyframes);
                }

                // Bind the clip and a game object
                if (clipBinding.ClipBindings.TryGetValue(animationChannel.NodeName, out var clips))
                {
                    clips.Add(clip);
                }
                else
                {
                    clipBinding.ClipBindings[animationChannel.NodeName] = new List<UnityEngine.AnimationClip> { clip };
                }
            }
        }

        /// <summary>
        /// Converts keys of an animated property to a vector animation in Unity.
        /// </summary>
        /// <typeparam name="T">Type of the animated property.</typeparam>
        /// <param name="keys">Keys of the animated property.</param>
        /// <param name="animationClip">Unity animation clip that will handle the animation.</param>
        /// <param name="propertyName">Name of the animated property.</param>
        /// <param name="convertFunc">Function that converts Assimp keys to a vector of Unity keyframes.</param>
        private static void ConvertVectorCurves<T>(IReadOnlyList<T> keys, UnityEngine.AnimationClip animationClip, string propertyName, Action<T, int, UnityEngine.Keyframe[], UnityEngine.Keyframe[], UnityEngine.Keyframe[]> convertFunc)
        {
            var xKeys = new UnityEngine.Keyframe[keys.Count];
            var yKeys = new UnityEngine.Keyframe[keys.Count];
            var zKeys = new UnityEngine.Keyframe[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                convertFunc(keys[i], i, xKeys, yKeys, zKeys);
            }

            var curve = new UnityEngine.AnimationCurve(xKeys);
            animationClip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.x", curve);

            curve = new UnityEngine.AnimationCurve(yKeys);
            animationClip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.y", curve);

            curve = new UnityEngine.AnimationCurve(zKeys);
            animationClip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.z", curve);
        }

        /// <summary>
        /// Helper function that converts a vector key to vector keyframes.
        /// </summary>
        /// <param name="key">The key of an animated property.</param>
        /// <param name="index">Index of the key.</param>
        /// <param name="xKeys">Keyframes of x element of a vector.</param>
        /// <param name="yKeys">Keyframes of y element of a vector.</param>
        /// <param name="zKeys">Keyframes of z element of a vector.</param>
        private static void ConvertVectorKeyToKeyframes(Assimp.VectorKey key, int index, UnityEngine.Keyframe[] xKeys, UnityEngine.Keyframe[] yKeys, UnityEngine.Keyframe[] zKeys)
        {
            xKeys[index] = new UnityEngine.Keyframe((float)key.Time, key.Value.X);
            yKeys[index] = new UnityEngine.Keyframe((float)key.Time, key.Value.Y);
            zKeys[index] = new UnityEngine.Keyframe((float)key.Time, key.Value.Z);
        }

        /// <summary>
        /// Helper function that converts quaternion key to vector keyframes.
        /// </summary>
        /// <param name="key">The key of an animated property.</param>
        /// <param name="index">Index of the key.</param>
        /// <param name="xKeys">Keyframes of x element of a vector.</param>
        /// <param name="yKeys">Keyframes of y element of a vector.</param>
        /// <param name="zKeys">Keyframes of z element of a vector.</param>
        private static void ConvertQuaternionKeyToKeyframes(Assimp.QuaternionKey key, int index,UnityEngine.Keyframe[] xKeys, UnityEngine.Keyframe[] yKeys, UnityEngine.Keyframe[] zKeys)
        {
            var quaternion = new UnityEngine.Quaternion(key.Value.X, key.Value.Y, key.Value.Z, key.Value.W);
            var eulerRotation = quaternion.eulerAngles;

            xKeys[index] = new UnityEngine.Keyframe((float)key.Time, eulerRotation.x);
            yKeys[index] = new UnityEngine.Keyframe((float)key.Time, eulerRotation.y);
            zKeys[index] = new UnityEngine.Keyframe((float)key.Time, eulerRotation.z);
        }
    }
}
