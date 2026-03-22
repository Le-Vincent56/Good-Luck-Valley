using System;
using System.Collections;
using UnityEngine;

namespace GoodLuckValley.Tests.PlayMode.SceneManagement
{
    /// <summary>
    /// Utility for bridging Unity 6 <see cref="Awaitable"/> to
    /// <see cref="IEnumerator"/> so async code can be tested in
    /// [UnityTest] play mode tests that require IEnumerator return types.
    /// </summary>
    public static class AwaitableTestHelper
    {
        /// <summary>
        /// Runs an Awaitable asynchronously and yields until it completes or throws an exception.
        /// </summary>
        /// <param name="awaitable">The Awaitable to execute asynchronously.</param>
        /// <returns>An IEnumerator that yields until the Awaitable completes or throws an exception.</returns>
        /// <exception cref="Exception">Thrown if the Awaitable encounters an error during execution.</exception>
        public static IEnumerator RunAwaitable(Awaitable awaitable)
        {
            bool completed = false;
            Exception caughtException = null;

            RunAsyncVoid(awaitable,
                () => completed = true,
                (Exception ex) =>
                {
                    caughtException = ex;
                    completed = true;
                }
            );

            while (!completed)
            {
                yield return null;
            }

            if (caughtException != null)
                throw caughtException;
        }

        /// <summary>
        /// Runs an Awaitable asynchronously and yields until it completes or throws, allowing testing of asynchronous operations in Unity tests.
        /// </summary>
        /// <param name="awaitable">The Awaitable to execute asynchronously.</param>
        /// <returns>An IEnumerator that yields until the Awaitable completes or throws an exception.</returns>
        public static IEnumerator RunAwaitable<T>(
            Awaitable<T> awaitable,
            Action<T> onResult
        )
        {
            bool completed = false;
            Exception caughtException = null;

            RunAsyncVoid(awaitable, onResult,
                () => completed = true,
                (Exception ex) =>
                {
                    caughtException = ex;
                    completed = true;
                }
            );

            while (!completed)
            {
                yield return null;
            }

            if (caughtException != null)
                throw caughtException;
        }

        /// <summary>
        /// Executes an Awaitable in an asynchronous context without returning a Task.
        /// Handles completion and exceptions through callbacks.
        /// </summary>
        /// <param name="awaitable">The Awaitable to run asynchronously.</param>
        /// <param name="onComplete">The callback to invoke upon successful completion of the Awaitable.</param>
        /// <param name="onError">The callback to invoke if the Awaitable throws an exception.</param>
        private static async void RunAsyncVoid(
            Awaitable awaitable,
            Action onComplete,
            Action<Exception> onError
        )
        {
            try
            {
                await awaitable;
                onComplete();
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        /// <summary>
        /// Executes an Awaitable of a generic type asynchronously, invoking
        /// specific callbacks based on the result of the operation.
        /// </summary>
        /// <typeparam name="T">The type of the Awaitable result.</typeparam>
        /// <param name="awaitable">The Awaitable to be executed asynchronously.</param>
        /// <param name="onResult">Callback invoked with the result of the Awaitable upon successful completion.</param>
        /// <param name="onComplete">Callback invoked upon successful completion of the Awaitable.</param>
        /// <param name="onError">Callback invoked with the exception if an error occurs during execution.</param>
        private static async void RunAsyncVoid<T>(
            Awaitable<T> awaitable,
            Action<T> onResult,
            Action onComplete,
            Action<Exception> onError
        )
        {
            try
            {
                T result = await awaitable;
                onResult(result);
                onComplete();
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }
    }
}