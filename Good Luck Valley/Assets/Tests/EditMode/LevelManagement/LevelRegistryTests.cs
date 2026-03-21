using System.Collections.Generic;
using NUnit.Framework;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEngine;

namespace GoodLuckValley.Tests.EditMode.LevelManagement
{
    [TestFixture]
    public class LevelRegistryTests
    {
        private LevelData _level01;
        private LevelData _level02;
        private LevelRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            _level01 = LevelData.CreateForTesting("Level01", "Forest Entrance", "Forest");
            _level02 = LevelData.CreateForTesting("Level02", "Forest Depths", "Forest");

            List<LevelData> levels = new List<LevelData> { _level01, _level02 };
            _registry = LevelRegistry.CreateForTesting(levels, _level01, "SpawnA");
        }

        [TearDown]
        public void TearDown()
        {
            if (_registry) Object.DestroyImmediate(_registry);
            if (_level01) Object.DestroyImmediate(_level01);
            if (_level02) Object.DestroyImmediate(_level02);
        }

        [Test]
        public void GetLevelBySceneID_KnownID_ReturnsCorrectLevel()
        {
            LevelData level = _registry.GetLevelBySceneID("Level01");

            Assert.IsNotNull(level);
            Assert.AreEqual("Level01", level.SceneID);
            Assert.AreEqual("Forest Entrance", level.DisplayName);
        }

        [Test]
        public void GetLevelBySceneID_UnknownId_ReturnsNull()
        {
            LevelData level = _registry.GetLevelBySceneID("NonExistent");

            Assert.IsNull(level);
        }

        [Test]
        public void GetLevelBySceneID_NullId_ReturnsNull()
        {
            LevelData level = _registry.GetLevelBySceneID(null);

            Assert.IsNull(level);
        }

        [Test]
        public void GetLevelBySceneID_EmptyId_ReturnsNull()
        {
            LevelData level = _registry.GetLevelBySceneID("");

            Assert.IsNull(level);
        }

        [Test]
        public void AllLevels_ReturnsAllConfiguredLevels()
        {
            Assert.AreEqual(2, _registry.AllLevels.Count);
        }

        [Test]
        public void StartingLevel_ReturnsConfiguredLevel()
        {
            Assert.AreSame(_level01, _registry.StartingLevel);
        }

        [Test]
        public void StartingSpawnPointId_ReturnsConfiguredValue()
        {
            Assert.AreEqual("SpawnA", _registry.StartingSpawnPointID);
        }

        [Test]
        public void LevelData_SpawnPointIds_ReturnsConfiguredIds()
        {
            LevelData level = LevelData.CreateForTesting(
                "TestLevel",
                spawnPointIDs: new List<string> { "SpawnA", "SpawnB" }
            );

            Assert.AreEqual(2, level.SpawnPointIDs.Count);
            Assert.AreEqual("SpawnA", level.SpawnPointIDs[0]);
            Assert.AreEqual("SpawnB", level.SpawnPointIDs[1]);

            Object.DestroyImmediate(level);
        }

        [Test]
        public void LevelData_AreaId_ReturnsConfiguredValue()
        {
            LevelData level = _registry.GetLevelBySceneID("Level01");

            Assert.AreEqual("Forest", level.AreaID);
        }
    }
}