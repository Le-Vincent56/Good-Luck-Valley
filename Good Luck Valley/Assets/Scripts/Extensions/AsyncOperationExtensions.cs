using System.Threading.Tasks;
using UnityEngine;

namespace GoodLuckValley.Extensions.Async
{
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// Extension method that converts an AsyncOperation into a Task
        /// </summary>
        /// <param name="asyncOperation">The AsyncOperation to convert</param>
        /// <returns>A task that represents the completion of the AsyncOperation</returns>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
