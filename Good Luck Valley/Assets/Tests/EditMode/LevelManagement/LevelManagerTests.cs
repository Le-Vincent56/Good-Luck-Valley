using System;
using System.Collections.Generic;
using NUnit.Framework;
using GoodLuckValley.World.LevelManagement.Data;
using GoodLuckValley.World.LevelManagement.Services;
using Object = UnityEngine.Object;

namespace GoodLuckValley.Tests.EditMode.LevelManagement
{
    [TestFixture]
    public class LevelManagerTests
    {
        private LevelData _startingLevel;
        private LevelRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _startingLevel = LevelData.CreateForTesting("Level01", "Test Level");
            List<LevelData> levels = new List<LevelData> { _startingLevel };
            _registry = LevelRegistry.CreateForTesting(levels, _startingLevel, "SpawnA");
        }

        [TearDown]
        public void TearDown()
        {
            if (_registry) Object.DestroyImmediate(_registry);
            if (_startingLevel) Object.DestroyImmediate(_startingLevel);
        }

        [Test]
        public void Constructor_CurrentLevel_IsNull()
        {
            MockSceneService sceneService = new MockSceneService();
            MockTransitionService transitionService = new MockTransitionService();

            LevelManager manager = new LevelManager(sceneService, transitionService, _registry);

            Assert.IsNull(manager.CurrentLevel);
        }

        [Test]
        public void Constructor_CurrentLevelScene_IsDefault()
        {
            MockSceneService sceneService = new MockSceneService();
            MockTransitionService transitionService = new MockTransitionService();

            LevelManager manager = new LevelManager(sceneService, transitionService, _registry);

            Assert.IsFalse(manager.CurrentLevelScene.IsValid());
        }

        [Test]
        public void Constructor_WithNullSceneService_ThrowsArgumentNullException()
        {
            MockTransitionService transitionService = new MockTransitionService();

            Assert.Throws<ArgumentNullException>(
                () => new LevelManager(null, transitionService, _registry)
            );
        }

        [Test]
        public void Constructor_WithNullTransitionService_ThrowsArgumentNullException()
        {
            MockSceneService sceneService = new MockSceneService();

            Assert.Throws<ArgumentNullException>(
                () => new LevelManager(sceneService, null, _registry)
            );
        }

        [Test]
        public void Constructor_WithNullLevelRegistry_ThrowsArgumentNullException()
        {
            MockSceneService sceneService = new MockSceneService();
            MockTransitionService transitionService = new MockTransitionService();

            Assert.Throws<ArgumentNullException>(
                () => new LevelManager(sceneService, transitionService, null)
            );
        }
    }
}