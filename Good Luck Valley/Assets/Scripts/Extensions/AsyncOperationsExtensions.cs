using System.Threading.Tasks;
using UnityEngine;

namespace GoodLuckValley.Extensions.AsyncOperations
{
    public static class AsyncOperationsExtensions
    {
        /// <summary>
        /// Convert an AsyncOperation into a Task
        /// </summary>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
