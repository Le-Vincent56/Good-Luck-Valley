using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using GoodLuckValley.Core.DI.Core;
using GoodLuckValley.Core.DI.Injection;
using GoodLuckValley.Core.DI.Interfaces;
using GoodLuckValley.Core.DI.Lifecycle;

namespace GoodLuckValley.Tests.PlayMode.DI
{
    [TestFixture]
    public class InjectionTimingTests
    {
        private Scene _testScene;
        private bool _sceneCreated;

        [TearDown]
        public void TearDown()
        {
            ContainerRegistry.Reset();
            InjectionCache.Clear();
            
            if(_sceneCreated && _testScene.IsValid() && _testScene.isLoaded)
                SceneManager.UnloadSceneAsync(_testScene);

            _sceneCreated = false;
        }

        private Scene CreateTestScene(string name)
        {
            _testScene = SceneManager.CreateScene(name);
            _sceneCreated = true;
            return _testScene;
        }
        
        // --- Test 1: [Inject] fields are null in Awake, populated in Start ---

        [UnityTest]
        public IEnumerator InjectFields_PopulatedBeforeStart()
        {
            Scene scene = CreateTestScene("Test_InjectTiming");
            GameObject go = new GameObject("TestAdapter");
            SceneManager.MoveGameObjectToScene(go, scene);
            TestPlayModeAdapter adapter = go.AddComponent<TestPlayModeAdapter>();
            
            ContainerRegistry.InstallScene(scene, new TestPlayModeAdapter.TestPlayModeInstaller());
            
            // Wait one frame: EarlyUpdate processes installation, then Start fires
            yield return null;
            
            Assert.IsTrue(adapter.WasNullInAwake, "Service should be null in Awake");
            Assert.IsTrue(adapter.WasSetInStart, "Service should be set by Start");
            Assert.IsNotNull("Service should be accessible start injection");
            Assert.AreEqual("Injected!", adapter.Service.GetValue());;
        }

        // --- Test 2: RegisterFromScene finds the correct component ---
        
        [UnityTest]
        public IEnumerator RegisterFromScene_FindsComponent()
        {
            Scene scene = CreateTestScene("Test_FindComponent");
            GameObject go = new GameObject("TestAdapter");
            SceneManager.MoveGameObjectToScene(go, scene);
            TestPlayModeAdapter adapter = go.AddComponent<TestPlayModeAdapter>();
            
            ContainerRegistry.InstallScene(scene, new TestPlayModeAdapter.TestPlayModeInstaller());

            yield return null;

            IContainer container = ContainerRegistry.GetContainerForScene(scene);
            Assert.IsNotNull(container, "Scene container should be registered");

            TestPlayModeAdapter resolved = container.Resolve<TestPlayModeAdapter>();
            Assert.AreSame(adapter, resolved, "Resolved adapter should be the same instance from the scene");
        }
        
        // --- Test 3: PrefabFactory instantiates and injects ---

        [UnityTest]
        public IEnumerator PrefabFactory_InstantiatesAndInjects()
        {
            // Create a "prefab" template (live object used as a clone source)
            GameObject prefabGo = new GameObject("PrefabTemplate");
            TestPlayModeAdapter prefabAdapter = prefabGo.AddComponent<TestPlayModeAdapter>();
            
            // Build container with service and prefab factory
            ContainerBuilder builder = new ContainerBuilder("Test_PrefabFactory");
            builder.RegisterSingleton<ITestPlayModeService, TestPlayModeService>();
            builder.RegisterPrefabFactory<TestPlayModeAdapter>(prefabAdapter);
            IContainer container = builder.Build();
            
            // Spawn via factory - injection happens synchronously in Create()
            IPrefabFactory<TestPlayModeAdapter> factory = container.Resolve<IPrefabFactory<TestPlayModeAdapter>>();
            TestPlayModeAdapter spawned = factory.Create(Vector3.zero, Quaternion.identity);

            Assert.IsNotNull(spawned, "Spawned instance should not be null");
            Assert.AreNotSame(prefabAdapter, spawned, "Spawned should be a new instance");
            Assert.IsNotNull(spawned.Service, "Spawned should have injected service");
            Assert.AreEqual("Injected!", spawned.Service.GetValue());
            
            // Cleanup
            UnityEngine.Object.Destroy(spawned.gameObject);
            UnityEngine.Object.Destroy(prefabGo);
            container.Dispose();

            yield return null;
        }
        
        // --- Test 4: Scene unload disposes container ---

        [UnityTest]
        public IEnumerator SceneUnload_DisposesContainer()
        {
            Scene scene = CreateTestScene("Test_SceneUnload");
            GameObject go = new GameObject("TestAdapter");
            SceneManager.MoveGameObjectToScene(go, scene);
            TestPlayModeAdapter adapter = go.AddComponent<TestPlayModeAdapter>();
            
            ContainerRegistry.InstallScene(scene, new TestPlayModeAdapter.TestPlayModeInstaller());
            
            yield return null;

            IContainer container = ContainerRegistry.GetContainerForScene(scene);
            Assert.IsNotNull(container, "Container should not exist after installation");
            
            // Unregister disposes the container
            ContainerRegistry.UnregisterSceneContainer(scene);

            Assert.Throws<ObjectDisposedException>(
                () => container.Resolve<ITestPlayModeService>(),
                "Container should be disposed after unregistration"
            );
        }
        
        // -- Test 5: PlayerLoop timing - Awake before injection before Start

        [UnityTest]
        public IEnumerator PlayerLoop_RunsAfterAwakeBeforeStart()
        {
            Scene scene = CreateTestScene("Test_PlayerLoopTiming");
            GameObject go = new GameObject("TestAdapter");
            SceneManager.MoveGameObjectToScene(go, scene);
            TestPlayModeAdapter adapter = go.AddComponent<TestPlayModeAdapter>();
            
            // Awake has already fired - verify service was null at that point
            Assert.IsTrue(adapter.WasNullInAwake, "Service must be null during Awake (before injection)");
            
            // Service should still be null right now (injection hasn't happened)
            Assert.IsNull(adapter.Service, "Service should be null before PlayerLoop processes installation");
            
            ContainerRegistry.InstallScene(scene, new TestPlayModeAdapter.TestPlayModeInstaller());
            
            // Wait one frame: EarlyUpdate processes installation, then Start fires
            yield return null;
            
            Assert.IsTrue(adapter.WasSetInStart, "Service must be set by Start (injection happened in EarlyUpdate)");
            Assert.IsNotNull(adapter.Service);
        }
    }
}