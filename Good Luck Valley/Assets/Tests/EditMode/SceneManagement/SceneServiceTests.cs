using System;
using System.Collections.Generic;
using NUnit.Framework;
using GoodLuckValley.Core.SceneManagement.Data;
using GoodLuckValley.Core.SceneManagement.Enums;
using GoodLuckValley.Core.SceneManagement.Services;
using Object = UnityEngine.Object;

namespace GoodLuckValley.Tests.EditMode.SceneManagement
{
    [TestFixture]
      public class SceneServiceTests
      {
          private SceneRegistry _registry;

          [SetUp]
          public void SetUp()
          {
              _registry = SceneRegistry.CreateForTesting(new List<SceneEntry>());
          }

          [TearDown]
          public void TearDown()
          {
              if (!_registry) return;
              
              Object.DestroyImmediate(_registry);
          }

          [Test]
          public void Constructor_InitialLoadState_IsIdle()
          {
              MockSceneLoader loader = new MockSceneLoader();
              TransitionService transition = new TransitionService();

              SceneService service = new SceneService(loader, _registry, transition);

              Assert.AreEqual(SceneLoadState.Idle, service.LoadState);
          }

          [Test]
          public void Constructor_InitialIsBusy_ReturnsFalse()
          {
              MockSceneLoader loader = new MockSceneLoader();
              TransitionService transition = new TransitionService();

              SceneService service = new SceneService(loader, _registry, transition);

              Assert.IsFalse(service.IsBusy);
          }

          [Test]
          public void Constructor_WithNullSceneLoader_ThrowsArgumentNullException()
          {
              TransitionService transition = new TransitionService();

              Assert.Throws<ArgumentNullException>(() => new SceneService(
                  null,
                  _registry, 
                  transition
              ));
          }

          [Test]
          public void Constructor_WithNullSceneRegistry_ThrowsArgumentNullException()
          {
              MockSceneLoader loader = new MockSceneLoader();
              TransitionService transition = new TransitionService();

              Assert.Throws<ArgumentNullException>(() => new SceneService(
                  loader, 
                  null, 
                  transition
              ));
          }

          [Test]
          public void Constructor_WithNullTransitionService_ThrowsArgumentNullException()
          {
              MockSceneLoader loader = new MockSceneLoader();

              Assert.Throws<ArgumentNullException>(() => new SceneService(
                  loader, 
                  _registry, 
                  null
              ));
          }
      }
}