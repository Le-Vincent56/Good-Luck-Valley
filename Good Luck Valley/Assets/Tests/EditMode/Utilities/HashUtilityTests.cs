using System;
using System.Collections.Generic;
using NUnit.Framework;
using GoodLuckValley.Core.Utilities;

namespace GoodLuckValley.Tests.EditMode.Utilities
{
    [TestFixture]
    public class HashUtilityTests
    {
        // --- Deterministic output ---

        [Test]
        public void ComputeStableHash_SameInput_ReturnsSameHash()
        {
            int first = HashUtility.ComputeStableHash("SampleLevel01");
            int second = HashUtility.ComputeStableHash("SampleLevel01");

            Assert.AreEqual(first, second);
        }

        [Test]
        public void ComputeStableHash_DifferentInputs_ReturnsDifferentHashes()
        {
            int hashA = HashUtility.ComputeStableHash("LevelA");
            int hashB = HashUtility.ComputeStableHash("LevelB");

            Assert.AreNotEqual(hashA, hashB);
        }

        [Test]
        public void ComputeStableHash_EmptyString_ReturnsConsistentValue()
        {
            int first = HashUtility.ComputeStableHash("");
            int second = HashUtility.ComputeStableHash("");

            Assert.AreEqual(first, second);
        }

        [Test]
        public void ComputeStableHash_KnownValue_MatchesExpected()
        {
            // FNV-1a of empty string = offset basis cast to int
            // 2166136261 as uint = -2128831035 as int
            int emptyHash = HashUtility.ComputeStableHash("");

            Assert.AreEqual(unchecked((int)2166136261u), emptyHash);
        }

        // --- Null handling ---

        [Test]
        public void ComputeStableHash_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HashUtility.ComputeStableHash(null)
            );
        }

        // --- Collision resistance ---

        [Test]
        public void ComputeStableHash_SampleSceneIds_NoCollisions()
        {
            string[] inputs = new string[]
            {
                "SampleLevel01", "SampleLevel02", "SampleLevel03",
                "ForestEntrance", "ForestDepths", "ForestClearing",
                "CaveEntrance", "CaveDepths", "CaveBoss",
                "MainMenu", "Transition", "GameplaySession",
                "SpawnPoint_01", "SpawnPoint_02", "SpawnPoint_03",
                "SpawnA", "SpawnB", "SpawnC"
            };

            HashSet<int> hashes = new HashSet<int>();

            for (int i = 0; i < inputs.Length; i++)
            {
                int hash = HashUtility.ComputeStableHash(inputs[i]);
                bool added = hashes.Add(hash);

                Assert.IsTrue(added, $"Collision detected: '{inputs[i]}' produced duplicate hash {hash}");
            }
        }

        // --- Edge cases ---

        [Test]
        public void ComputeStableHash_WhitespaceString_ReturnsValidHash()
        {
            int hash = HashUtility.ComputeStableHash(" ");

            Assert.AreNotEqual(0, hash);
        }

        [Test]
        public void ComputeStableHash_UnicodeInput_ReturnsDeterministicHash()
        {
            int first = HashUtility.ComputeStableHash("Lëvel_01");
            int second = HashUtility.ComputeStableHash("Lëvel_01");

            Assert.AreEqual(first, second);
        }
    }
}