using System;
using System.Collections.Generic;

namespace ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion
{
    /// <summary>
    /// Convertor of an Assimp animation to Unity.
    /// </summary>
    public static class AnimationConverter
    {
        public struct KeyframeVector
        {
            public readonly UnityEngine.Keyframe X;
            public readonly UnityEngine.Keyframe Y;
            public readonly UnityEngine.Keyframe Z;

            public KeyframeVector(UnityEngine.Keyframe x, UnityEngine.Keyframe y, UnityEngine.Keyframe z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public struct CurveVector
        {
            public UnityEngine.AnimationCurve X;
            public UnityEngine.AnimationCurve Y;
            public UnityEngine.AnimationCurve Z;
            
            public CurveVector(UnityEngine.AnimationCurve x, UnityEngine.AnimationCurve y, UnityEngine.AnimationCurve z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
        
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

            if (animation.HasMeshAnimations)
            {
                for (var i = 0; i < animation.MeshAnimationChannelCount; i++)
                {
                    var animationChannel = animation.MeshAnimationChannels[i];
                    ConvertVertexAnimation(animationChannel);
                }
            }

            if (animation.HasNodeAnimations)
            {
                for (var i = 0; i < animation.NodeAnimationChannelCount; i++)
                {
                    var animationChannel = animation.NodeAnimationChannels[i];
                    var animationClip = ConvertKeyframeAnimation(animationChannel);
                    BindAnimationToGameObject(clipBinding, animationClip, animationChannel.NodeName);
                }
            }

            return clipBinding;
        }

        public static void BindAnimationToGameObject(NodeClipBinding clipBinding, UnityEngine.AnimationClip animationClip, string gameObjectName)
        {
            // Bind the clip and a game object
            if (clipBinding.ClipBindings.TryGetValue(gameObjectName, out var clips))
            {
                clips.Add(animationClip);
            }
            else
            {
                clipBinding.ClipBindings[gameObjectName] = new List<UnityEngine.AnimationClip> { animationClip };
            }
        }

        public static void ConvertVertexAnimation(Assimp.MeshAnimationChannel animationChannel)
        {
            // TODO: animation of mesh vertices
            UnityEngine.Debug.LogError("Vertex animation is not supported");
        }

        public static UnityEngine.AnimationClip ConvertKeyframeAnimation(Assimp.NodeAnimationChannel animationChannel)
        {
            // SetCurve can be called only on legacy animation clip in runtime.
            var clip = new UnityEngine.AnimationClip { legacy = true };

            // Set position keys
            if (animationChannel.HasPositionKeys)
            {
                var curveVector = ConvertKeysToCurves(animationChannel.PositionKeys, ConvertVectorKeyToKeyframes);
                SetVectorCurves(clip, $"localPosition", curveVector);
            }

            // Set rotation keys
            if (animationChannel.HasRotationKeys)
            {
                var curveVector = ConvertKeysToCurves(animationChannel.RotationKeys, ConvertQuaternionKeyToKeyframes);
                SetVectorCurves(clip, $"localRotation", curveVector);
            }

            // Set scaling keys
            if (animationChannel.HasScalingKeys)
            {
                var curveVector = ConvertKeysToCurves(animationChannel.ScalingKeys, ConvertVectorKeyToKeyframes);
                SetVectorCurves(clip, $"localScale", curveVector);
            }

            return clip;
        }

        public static void SetVectorCurves(UnityEngine.AnimationClip clip, string propertyName, CurveVector curves)
        {
            clip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.x", curves.X);
            clip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.y", curves.Y);
            clip.SetCurve("", typeof(UnityEngine.Transform), $"{propertyName}.z", curves.Z);
        }

        /// <summary>
        /// Converts keys of an animated property to a vector animation in Unity.
        /// </summary>
        /// <typeparam name="T">Type of the animated property.</typeparam>
        /// <param name="keys">Keys of the animated property.</param>
        /// <param name="keyConvertFunc">Function that converts Assimp keys to a vector of Unity keyframes.</param>
        public static CurveVector ConvertKeysToCurves<T>(IReadOnlyList<T> keys, Func<T, KeyframeVector> keyConvertFunc)
        {
            var xKeys = new UnityEngine.Keyframe[keys.Count];
            var yKeys = new UnityEngine.Keyframe[keys.Count];
            var zKeys = new UnityEngine.Keyframe[keys.Count];

            for (var i = 0; i < keys.Count; i++)
            {
                var keyframeVector = keyConvertFunc(keys[i]);
                xKeys[i] = keyframeVector.X;
                yKeys[i] = keyframeVector.Y;
                zKeys[i] = keyframeVector.Z;
            }

            return new CurveVector(
                new UnityEngine.AnimationCurve(xKeys), 
                new UnityEngine.AnimationCurve(yKeys), 
                new UnityEngine.AnimationCurve(zKeys));
        }

        /// <summary>
        /// Helper function that converts a vector key to vector keyframes.
        /// </summary>
        /// <param name="key">The key of an animated property.</param>
        public static KeyframeVector ConvertVectorKeyToKeyframes(Assimp.VectorKey key)
        {
            return new KeyframeVector(
                new UnityEngine.Keyframe((float)key.Time, key.Value.X),
                new UnityEngine.Keyframe((float)key.Time, key.Value.Y),
                new UnityEngine.Keyframe((float)key.Time, key.Value.Z));
        }

        /// <summary>
        /// Helper function that converts quaternion key to vector keyframes.
        /// </summary>
        /// <param name="key">The key of an animated property.</param>
        public static KeyframeVector ConvertQuaternionKeyToKeyframes(Assimp.QuaternionKey key)
        {
            var quaternion = new UnityEngine.Quaternion(key.Value.X, key.Value.Y, key.Value.Z, key.Value.W);
            var eulerRotation = quaternion.eulerAngles;

            return new KeyframeVector(
                new UnityEngine.Keyframe((float)key.Time, eulerRotation.x),
                new UnityEngine.Keyframe((float)key.Time, eulerRotation.y),
                new UnityEngine.Keyframe((float)key.Time, eulerRotation.z));
        }
    }
}