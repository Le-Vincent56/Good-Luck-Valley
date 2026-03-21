using System.Collections.Generic;
using System.Runtime.Remoting;
using NUnit.Framework;
using GoodLuckValley.Core.SceneManagement.Data;
using UnityEngine;

namespace GoodLuckValley.Tests.EditMode.SceneManagement
{
    [TestFixture]
    public class SceneRegistryTests
    {
        private SceneRegistry _registry;

        [SetUp]
        public void SetUp()
        {
            List<SceneEntry> entries = new List<SceneEntry>
            {
                new SceneEntry("MainMenu", "Some.MainMenuInstaller", true, false),
                new SceneEntry("Transition", null, false, true),
                new SceneEntry("Level01", "Some.Level01Installer", true, false)
            };

            _registry = SceneRegistry.CreateForTesting(entries, "Transition", "MainMenu");
        }

        [TearDown]
        public void TearDown()
        {
            if (!_registry) return;

            Object.DestroyImmediate(_registry);
        }

        [Test]
        public void GetEntry_KnownID_ReturnsCorrectEntry()
        {
            SceneEntry entry = _registry.GetEntry("MainMenu");
            
            Assert.IsNotNull(entry);
            Assert.AreEqual("MainMenu", entry.SceneID);
        }

        [Test]
        public void GetEntry_ReturnsEntryWithCorrectInstallerConfig()
        {
            SceneEntry entry = _registry.GetEntry("MainMenu");
            
            Assert.AreEqual("Some.MainMenuInstaller", entry.InstallerTypeName);
            Assert.IsTrue(entry.IsScoped);
            Assert.IsFalse(entry.SkipContainerInstallation);
        }

        [Test]
        public void GetEntry_UnknownID_ReturnsNull()
        {
            SceneEntry entry = _registry.GetEntry("Unknown");
            
            Assert.IsNull(entry);
        }

        [Test]
        public void GetEntry_NullID_ReturnsNull()
        {
            SceneEntry entry = _registry.GetEntry(null);
            
            Assert.IsNull(entry);
        }

        [Test]
        public void GetEntry_EmptyID_ReturnsNull()
        {
            SceneEntry entry = _registry.GetEntry("");
            
            Assert.IsNull(entry);
        }

        [Test]
        public void Entries_ReturnsAllConfiguredEntries()
        {
            Assert.AreEqual(3, _registry.Entries.Count);
        }

        [Test]
        public void TransitionSceneID_ReturnsConfiguredValue()
        {
            Assert.AreEqual("Transition", _registry.TransitionSceneID);
        }

        [Test]
        public void InitialSceneID_ReturnsConfiguredValue()
        {
            Assert.AreEqual("MainMenu", _registry.InitialSceneID);
        }

        [Test]
        public void GetEntry_SkipContainerInstallation_ReturnsCorrectFlag()
        {
            SceneEntry entry = _registry.GetEntry("Transition");
            
            Assert.IsNotNull(entry);
            Assert.IsTrue(entry.SkipContainerInstallation);
        }

        [Test]
        public void SceneEntry_GetAddress_WithNoReference_FallsBackToSceneID()
        {
            SceneEntry entry = _registry.GetEntry("Transition");
            
            Assert.AreEqual("Transition", entry.GetAddress());
        }
    }
}