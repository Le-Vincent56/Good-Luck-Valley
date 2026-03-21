using GoodLuckValley.Core.DI.Attributes;
using GoodLuckValley.Core.DI.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.Tests.PlayMode.DI
{
    // --- Test service for injection verification ---

    public interface ITestPlayModeService
    {
        string GetValue();
    }

    public class TestPlayModeService : ITestPlayModeService
    {
        public string GetValue() => "Injected!";
    }
    
    // --- Test adapter that tracks injection timing ---
    public class TestPlayModeAdapter : MonoBehaviour
    {
        [Inject] private ITestPlayModeService _service;
        
        /// <summary>
        /// True if _service was null when Awake fired (expected - injection hasn't happened yet). 
        /// </summary>
        public bool WasNullInAwake { get; private set; }
        
        /// <summary>
        /// True if _service was set when Start fired (expected - injection happened in EarlyUpdate).
        /// </summary>
        public bool WasSetInStart { get; private set; }
        
        public bool StartCalled { get; private set; }
        
        /// <summary>
        /// The injected service, accessible after injection.
        /// </summary>
        public ITestPlayModeService Service => _service;

        private void Awake() => WasNullInAwake = _service == null;

        private void Start()
        {
            StartCalled = true;
            WasSetInStart = _service != null;
        }
        
        // --- Installer for test scenes ---
        
        public class TestPlayModeInstaller : IInstaller
        {
            public void Install(IContainerBuilder builder, Scene scene)
            {
                builder.RegisterSingleton<ITestPlayModeService, TestPlayModeService>();
                builder.RegisterFromScene<TestPlayModeAdapter>(scene);
            }
        }
    }
}