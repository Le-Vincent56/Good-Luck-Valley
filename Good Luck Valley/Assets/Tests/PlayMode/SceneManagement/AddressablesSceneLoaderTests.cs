using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GoodLuckValley.Tests.PlayMode.SceneManagement
{
    /// <summary>
    /// Play mode tests for <see cref="Core.SceneManagement.Services.AddressablesSceneLoader"/>.
    /// Requires Addressable test scenes to be configured in the project. Tests are ignored until assets are available.
    /// </summary>
    [TestFixture]
    public class AddressablesSceneLoaderTests
    {
        // NOTE: These tests require Addressable scene assets to be configured.
        // Create the following test scenes and mark them as Addressable:
        //   - "TestScene_LoadUnload" (empty scene for load/unload testing)
        //   - "TestScene_Additive"   (empty scene for additive loading)
        // Then remove the [Ignore] attributes.

        [UnityTest]
        [Ignore("Requires Addressable test scenes — configured later")]
        public IEnumerator LoadSceneAsync_ValidAddress_ReturnsSuccess()
        {
            // Arrange: AddressablesSceneLoader instance
            // Act: LoadSceneAsync("TestScene_LoadUnload", SceneLoadMode.Additive)
            // Assert: result.Success == true, result.Scene.IsValid()
            yield return null;
        }

        [UnityTest]
        [Ignore("Requires Addressable test scenes — configured later")]
        public IEnumerator LoadSceneAsync_InvalidAddress_ReturnsFailure()
        {
            // Arrange: AddressablesSceneLoader instance
            // Act: LoadSceneAsync("NonExistentScene", SceneLoadMode.Additive)
            // Assert: result.Success == false, result.ErrorMessage is not empty
            yield return null;
        }

        [UnityTest]
        [Ignore("Requires Addressable test scenes — configured later")]
        public IEnumerator UnloadSceneAsync_PreviouslyLoadedScene_ReturnsTrue()
        {
            // Arrange: Load a scene, store the Scene reference
            // Act: UnloadSceneAsync(scene)
            // Assert: returns true
            yield return null;
        }

        [UnityTest]
        [Ignore("Requires Addressable test scenes — configured later")]
        public IEnumerator UnloadSceneAsync_UnknownScene_ReturnsFalse()
        {
            // Arrange: AddressablesSceneLoader instance, no scenes loaded
            // Act: UnloadSceneAsync(default(Scene))
            // Assert: returns false
            yield return null;
        }

        [UnityTest]
        [Ignore("Requires Addressable test scenes — configured later")]
        public IEnumerator LoadSceneAsync_Additive_DoesNotUnloadExistingScenes()
        {
            // Arrange: Load first scene
            // Act: Load second scene additively
            // Assert: both scenes are loaded
            yield return null;
        }
    }
}