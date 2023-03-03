using System.Collections.Generic;
using Assimp;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Models.MeshImport.Conversion;
using Animation = Assimp.Animation;
using Quaternion = Assimp.Quaternion;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Models.MeshImport.Conversion
{
    [RequiresPlayMode(false)]
    [TestFixture]
    public class AnimationConverterTest
    {
        #region ConvertQuaternionKeyToKeyframes
        
        [TestCase(0, 0, 0, 0, 1, 0, 0, 0)]
        public void ConvertQuaternionKeyToKeyframes_DifferentQuaternions_ReturnCorrectEulerAngles(int time, float x, float y, float z, float w, float expectedX, float expectedY, float expectedZ)
        {
            var key = new QuaternionKey(time, new Quaternion(w, x, y, z));

            var keyframes = AnimationConverter.ConvertQuaternionKeyToKeyframes(key);
            
            AssertKeyframeVector(keyframes.X, keyframes.Y, keyframes.Z, time, expectedX, expectedY, expectedZ);
        }
        
        #endregion
        
        #region ConvertVectorKeyToKeyframes
        
        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 2, 3, 4)]
        public void ConvertVectorKeyToKeyframes_DifferentVectors_ReturnsCorrectValues(int time, float x, float y, float z)
        {
            var key = new VectorKey(time, new Vector3D(x, y, z));

            var keyframes = AnimationConverter.ConvertVectorKeyToKeyframes(key);
            
            AssertKeyframeVector(keyframes.X, keyframes.Y, keyframes.Z, time, x, y, z);
        }

        #endregion
        
        #region ConvertKeysToCurves

        [Test]
        public void ConvertKeysToCurves_EmptyVectorKeys_CurvesAreEmpty()
        {
            var keys = new List<VectorKey>();
            
            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertVectorKeyToKeyframes);

            AssertCurvesLength(curves, 0);
        }
        
        [Test]
        public void ConvertKeysToCurves_SingleVectorKey_CurvesHasCorrectKeyframes()
        {
            var keys = new List<VectorKey>
            {
                new(0, new Vector3D(0, 0, 0))
            };
            
            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertVectorKeyToKeyframes);

            AssertKeyframeVector(curves.X[0], curves.Y[0], curves.Z[0], 0, 0, 0, 0);
        }

        [Test]
        public void ConvertKeysToCurves_MultipleVectorKeys_CurvesHasCorrectKeyframes()
        {
            var keys = new List<VectorKey>
            {
                new(1, new Vector3D(2, 3, 4)),
                new(5, new Vector3D(6, 7, 8))
            };

            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertVectorKeyToKeyframes);
            
            AssertKeyframeVector(curves.X[0], curves.Y[0], curves.Z[0], 1, 2, 3, 4);
            AssertKeyframeVector(curves.X[1], curves.Y[1], curves.Z[1], 5, 6, 7, 8);
        }
        
        [Test]
        public void ConvertKeysToCurves_EmptyQuaternionKeys_CurvesAreEmpty()
        {
            var keys = new List<QuaternionKey>();
            
            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertQuaternionKeyToKeyframes);

            AssertCurvesLength(curves, 0);
        }

        [Test]
        public void ConvertKeysToCurves_SingleQuaternionKey_CurvesHasCorrectKeyframes()
        {
            var keys = new List<QuaternionKey>
            {
                new(0, new Quaternion(1, 0, 0, 0))
            };
            
            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertQuaternionKeyToKeyframes);

            AssertKeyframeVector(curves.X[0], curves.Y[0], curves.Z[0], 0, 0, 0, 0);
        }
        
        [Test]
        public void ConvertKeysToCurves_MultipleQuaternionKeys_CurvesHasCorrectKeyframes()
        {
            var keys = new List<QuaternionKey>
            {
                new(1, new Quaternion(1, 0, 0, 0)),
                new(2, new Quaternion(1, 0, 0, 0))
            };

            var curves = AnimationConverter.ConvertKeysToCurves(keys, AnimationConverter.ConvertQuaternionKeyToKeyframes);

            AssertKeyframeVector(curves.X[0], curves.Y[0], curves.Z[0], 1, 0, 0, 0);
            AssertKeyframeVector(curves.X[1], curves.Y[1], curves.Z[1], 2, 0, 0, 0);
        }
        
        #endregion
        
        #region SetVectorCurves

        [Test]
        public void SetVectorCurves_PropertyName_HasCorrectFormat()
        {
            var (propertyName, animationClip, curves) = InitializeSetVectorCurves();

            AnimationConverter.SetVectorCurves(animationClip, propertyName, curves);
            var bindings = AnimationUtility.GetCurveBindings(animationClip);

            Assert.That(bindings[0].propertyName, Is.EqualTo($"{propertyName}.x"));
            Assert.That(bindings[1].propertyName, Is.EqualTo($"{propertyName}.y"));
            Assert.That(bindings[2].propertyName, Is.EqualTo($"{propertyName}.z"));
        }

        [Test]
        public void SetVectorCurves_RelativePath_IsEmpty()
        {
            var (propertyName, animationClip, curves) = InitializeSetVectorCurves();

            AnimationConverter.SetVectorCurves(animationClip, propertyName, curves);
            var bindings = AnimationUtility.GetCurveBindings(animationClip);

            Assert.That(bindings[0].path, Is.EqualTo(""));
            Assert.That(bindings[1].path, Is.EqualTo(""));
            Assert.That(bindings[2].path, Is.EqualTo(""));
        }
        
        [Test]
        public void SetVectorCurves_ObjectType_IsTransform()
        {
            var (propertyName, animationClip, curves) = InitializeSetVectorCurves();

            AnimationConverter.SetVectorCurves(animationClip, propertyName, curves);
            var bindings = AnimationUtility.GetCurveBindings(animationClip);

            Assert.That(bindings[0].type, Is.EqualTo(typeof(Transform)));
            Assert.That(bindings[1].type, Is.EqualTo(typeof(Transform)));
            Assert.That(bindings[2].type, Is.EqualTo(typeof(Transform)));
        }
        
        [Test]
        public void SetVectorCurves_CurveValues_AreTheSame()
        {
            var (propertyName, animationClip, curves) = InitializeSetVectorCurves();
            
            AnimationConverter.SetVectorCurves(animationClip, propertyName, curves);
            var bindings = AnimationUtility.GetCurveBindings(animationClip);
            var (curveX, curveY, curveZ) = GetCurvesFromClip(animationClip, bindings);

            Assert.That(curveX, Is.EqualTo(curves.X));
            Assert.That(curveY, Is.EqualTo(curves.Y));
            Assert.That(curveZ, Is.EqualTo(curves.Z));
        }

        private static (string propertyName, AnimationClip animationClip, AnimationConverter.CurveVector curves) InitializeSetVectorCurves()
        {
            const string propertyName = "propertyName";
            var animationClip = new AnimationClip();
            var curves = new AnimationConverter.CurveVector(
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve());
            
            return (propertyName, animationClip, curves);
        }
        
        #endregion
        
        #region ConvertKeyframeAnimation

        [Test]
        public void ConvertKeyframeAnimation_EmptyKeys_EmptyClip()
        {
            var animationChannel = new NodeAnimationChannel();
            
            var clip = AnimationConverter.ConvertKeyframeAnimation(animationChannel);
            var bindings = AnimationUtility.GetCurveBindings(clip);
            
            Assert.That(bindings.Length, Is.EqualTo(0));
        }
        
        [Test]
        public void ConvertKeyframeAnimation_PositionKeys_CorrectKeyframes()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.PositionKeys.Add(new VectorKey(1, new Vector3D(2, 3, 4)));

            TestKeyframeAnimation(animationChannel, 1, 2, 3, 4);
        }
        
        [Test]
        public void ConvertKeyframeAnimation_RotationKeys_CorrectKeyframes()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.RotationKeys.Add(new QuaternionKey(1, new Quaternion(1, 0, 0, 0)));

            TestKeyframeAnimation(animationChannel, 1, 0, 0, 0);
        }
        
        [Test]
        public void ConvertKeyframeAnimation_ScaleKeys_CorrectKeyframes()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.ScalingKeys.Add(new VectorKey(1, new Vector3D(2, 3, 4)));

            TestKeyframeAnimation(animationChannel, 1, 2, 3, 4);
        }
        
        [Test]
        public void ConvertKeyframeAnimation_PositionKeys_CorrectPropertyBinding()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.PositionKeys.Add(new VectorKey(1, new Vector3D(2, 3, 4)));
            
            TestCurvePropertyBinding(animationChannel, "m_LocalPosition");
        }
        
        [Test]
        public void ConvertKeyframeAnimation_RotationKeys_CorrectPropertyBinding()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.RotationKeys.Add(new QuaternionKey(1, new Quaternion(1, 0, 0, 0)));
            
            TestCurvePropertyBinding(animationChannel, "m_LocalRotation");
        }
        
        [Test]
        public void ConvertKeyframeAnimation_ScaleKeys_CorrectPropertyBinding()
        {
            var animationChannel = new NodeAnimationChannel();
            animationChannel.ScalingKeys.Add(new VectorKey(1, new Vector3D(2, 3, 4)));

            TestCurvePropertyBinding(animationChannel, "m_LocalScale");
        }

        private static void TestCurvePropertyBinding(NodeAnimationChannel animationChannel, string expectedPropertyName)
        {
            var clip = AnimationConverter.ConvertKeyframeAnimation(animationChannel);
            var bindings = AnimationUtility.GetCurveBindings(clip);

            AssertCurveBinding(bindings[0], $"{expectedPropertyName}.x");
            AssertCurveBinding(bindings[1], $"{expectedPropertyName}.y");
            AssertCurveBinding(bindings[2], $"{expectedPropertyName}.z");
        }

        private static void TestKeyframeAnimation(NodeAnimationChannel animationChannel, int time, float expectedX, float expectedY, float expectedZ)
        {
            var clip = AnimationConverter.ConvertKeyframeAnimation(animationChannel);

            var bindings = AnimationUtility.GetCurveBindings(clip);
            var (curveX, curveY, curveZ) = GetCurvesFromClip(clip, bindings);
            
            AssertKeyframeVector(curveX[0], curveY[0], curveZ[0], time, expectedX, expectedY, expectedZ);
        }

        #endregion
        
        #region BindAnimationToGameObject

        [Test]
        public void BindAnimationToGameObject_EmptyBinding_AddsKey()
        {
            var clipBinding = new NodeClipBinding();
            var animationClip = new AnimationClip();
            const string gameObjectName = "name";
            
            AnimationConverter.BindAnimationToGameObject(clipBinding, animationClip, gameObjectName);
            
            Assert.That(clipBinding.ClipBindings.ContainsKey(gameObjectName), Is.True);
        }
        
        [Test]
        public void BindAnimationToGameObject_EmptyBinding_AddsClip()
        {
            const string gameObjectName = "name";
            var clipBinding = new NodeClipBinding();
            var animationClip = new AnimationClip();

            AnimationConverter.BindAnimationToGameObject(clipBinding, animationClip, gameObjectName);
            
            Assert.That(clipBinding.ClipBindings[gameObjectName], Is.EqualTo(new List<AnimationClip> { animationClip }));
        }
        
        [Test]
        public void BindAnimationToGameObject_ExistingBinding_AddsNewClip()
        {
            const string gameObjectName = "name";
            var animationClip = new AnimationClip();
            var newAnimationClip = new AnimationClip();
            var clipBinding = new NodeClipBinding { ClipBindings = { [gameObjectName] = new List<AnimationClip> { animationClip } } };

            AnimationConverter.BindAnimationToGameObject(clipBinding, newAnimationClip, gameObjectName);
            
            Assert.That(clipBinding.ClipBindings[gameObjectName], Is.EqualTo(new List<AnimationClip> { animationClip, newAnimationClip}));
        }
        
        #endregion
        
        #region ConvertToUnity

        [Test]
        public void ConvertToUnity_NoAnimation_EmptyBinding()
        {
            var animation = new Animation();

            var clipBinding = AnimationConverter.ConvertToUnity(animation);
            
            Assert.That(clipBinding.ClipBindings.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void ConvertToUnity_NodeAnimation_IsAddedToBinding()
        {
            var animation = new Animation();
            
            var animationChannel = new NodeAnimationChannel { NodeName = "gameObject" };
            animation.NodeAnimationChannels.Add(animationChannel);
            
            var clipBinding = AnimationConverter.ConvertToUnity(animation);
            
            Assert.That(clipBinding.ClipBindings.ContainsKey(animationChannel.NodeName), Is.True);
        }
        
        [Test]
        public void ConvertToUnity_MultipleNodeAnimations_AreAddedToBinding()
        {
            var animation = new Animation();
            
            var animationChannel = new NodeAnimationChannel { NodeName = "gameObject" };
            animation.NodeAnimationChannels.Add(animationChannel);
            var animationChannel2 = new NodeAnimationChannel { NodeName = "gameObject2" };
            animation.NodeAnimationChannels.Add(animationChannel2);
            
            var clipBinding = AnimationConverter.ConvertToUnity(animation);
            
            Assert.That(clipBinding.ClipBindings.ContainsKey(animationChannel.NodeName), Is.True);
            Assert.That(clipBinding.ClipBindings.ContainsKey(animationChannel2.NodeName), Is.True);
        }
        
        #endregion
        
        private static (AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ) GetCurvesFromClip(
            AnimationClip animationClip, IReadOnlyList<EditorCurveBinding> bindings)
        {
            var curveX = AnimationUtility.GetEditorCurve(animationClip, bindings[0]);
            var curveY = AnimationUtility.GetEditorCurve(animationClip, bindings[1]);
            var curveZ = AnimationUtility.GetEditorCurve(animationClip, bindings[2]);
            return (curveX, curveY, curveZ);
        }
        
        private static void AssertCurvesLength(AnimationConverter.CurveVector curves, int length)
        {
            Assert.That(curves.X.length, Is.EqualTo(length));
            Assert.That(curves.Y.length, Is.EqualTo(length));
            Assert.That(curves.Z.length, Is.EqualTo(length));
        }
        
        private static void AssertCurveBinding(EditorCurveBinding binding, string expectedPropertyName)
        {
            Assert.That(binding.path, Is.EqualTo(""));
            Assert.That(binding.propertyName, Is.EqualTo(expectedPropertyName));
            Assert.That(binding.type, Is.EqualTo(typeof(Transform)));
        }
        
        private static void AssertKeyframeVector(Keyframe keyframeX, Keyframe keyframeY, Keyframe keyframeZ, 
            int time, float x, float y, float z)
        {
            AssertKeyframe(keyframeX, time, x);
            AssertKeyframe(keyframeY, time, y);
            AssertKeyframe(keyframeZ, time, z);
        }

        private static void AssertKeyframe(Keyframe keyframe, int time, float x)
        {
            Assert.That(keyframe.time, Is.EqualTo(time));
            Assert.That(keyframe.value, Is.EqualTo(x));
        }
    }
}