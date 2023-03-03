using System;
using System.Collections;
using NUnit.Framework;

namespace ZCU.TechnologyLab.Common.Unity.Tests.Library
{
    public static class AssertCoroutine
    {
        public static IEnumerator AssertThrows<T>(IEnumerator enumerator) where T : Exception
        {
            while (true)
            {
                object current;
                try
                {
                    if (enumerator.MoveNext() == false)
                    {
                        break;
                    }

                    current = enumerator.Current;
                }
                catch (T)
                {
                    Assert.Pass();
                    yield break;
                }

                yield return current;
            }

            Assert.Fail($"Expected exception {typeof(T)} was not thrown.");
        }
    }
}