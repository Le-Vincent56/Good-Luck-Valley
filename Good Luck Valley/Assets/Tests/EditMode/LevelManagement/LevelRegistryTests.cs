using System.Collections.Generic;
using GoodLuckValley.Core.Utilities;
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
            _level01 = LevelData.CreateForTesting(
                "Level01",
                "Forest Entrance",
                "Forest",
                stableID: HashUtility.ComputeStableHash("Level01")
            );
            _level02 = LevelData.CreateForTesting(
                "Level02",
                "Forest Depths",
                "Forest",
                stableID: HashUtility.ComputeStableHash("Level02")
            );

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
            List<BakedSpawnPoint> spawnPoints = new List<BakedSpawnPoint>
            {
                new BakedSpawnPoint("SpawnA", HashUtility.ComputeStableHash("SpawnA"), Vector2.zero, true),
                new BakedSpawnPoint("SpawnB", HashUtility.ComputeStableHash("SpawnB"), new Vector2(5f, 0f), false)
            };

            LevelData level = LevelData.CreateForTesting(
                "TestLevel",
                bakedSpawnPoints: spawnPoints
            );

            Assert.AreEqual(2, level.BakedSpawnPoints.Count);
            Assert.AreEqual("SpawnA", level.BakedSpawnPoints[0].ID);
            Assert.AreEqual("SpawnB", level.BakedSpawnPoints[1].ID);
            Assert.AreEqual(Vector2.zero, level.BakedSpawnPoints[0].Position);
            Assert.IsTrue(level.BakedSpawnPoints[0].FaceRight);
            Assert.IsFalse(level.BakedSpawnPoints[1].FaceRight);

            Object.DestroyImmediate(level);
        }

        [Test]
        public void LevelData_AreaId_ReturnsConfiguredValue()
        {
            LevelData level = _registry.GetLevelBySceneID("Level01");

            Assert.AreEqual("Forest", level.AreaID);
        }

        [Test]
        public void GetLevelByStableID_KnownID_ReturnsCorrectLevel()
        {
            int stableID = HashUtility.ComputeStableHash("Level01");

            LevelData level = _registry.GetLevelByStableID(stableID);

            Assert.IsNotNull(level);
            Assert.AreEqual("Level01", level.SceneID);
            Assert.AreEqual("Forest Entrance", level.DisplayName);
        }

        [Test]
        public void GetLevelByStableID_UnknownID_ReturnsNull()
        {
            LevelData level = _registry.GetLevelByStableID(999999);

            Assert.IsNull(level);
        }

        [Test]
        public void GetLevelByStableID_Zero_ReturnsNull()
        {
            LevelData level = _registry.GetLevelByStableID(0);

            Assert.IsNull(level);
        }
    }
}