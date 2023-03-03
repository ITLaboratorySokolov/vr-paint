using System.Collections;
using System.Threading.Tasks;

namespace ZCU.TechnologyLab.Common.Unity.Models.Utility
{
    public static class TaskExtension
    {
        public static IEnumerator AsCoroutine(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception!;
            }
        }
    }
}