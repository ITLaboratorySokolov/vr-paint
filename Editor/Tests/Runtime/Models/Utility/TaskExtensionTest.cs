using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ZCU.TechnologyLab.Common.Unity.Models.Utility;
using ZCU.TechnologyLab.Common.Unity.Tests.Library;

namespace Tests.Runtime.Models.Utility
{
    public class TaskExtensionTest
    {
        [UnityTest]
        public IEnumerator AsCoroutine_DoLongCalculation_WaitsForResult()
        {
            var waited = false;
            yield return Task.Run(async () =>
            {
                await Task.Delay(5000); // simulates long calculation
                waited = true;
            }).AsCoroutine();

            Assert.That(waited, Is.True);
        }

        [UnityTest]
        public IEnumerator AsCoroutine_ThrowException_IsPropagatedFromTask()
        {
            yield return AssertCoroutine.AssertThrows<Exception>(Task.Run(() => throw new Exception()).AsCoroutine());
        }
    }
}