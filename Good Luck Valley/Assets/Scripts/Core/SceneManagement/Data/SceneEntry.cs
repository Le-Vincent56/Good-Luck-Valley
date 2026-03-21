using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoodLuckValley.Core.SceneManagement.Data
{
    /// <summary>
    /// Serializable entry mapping a scene ID to its Addressable reference
    /// and DI installer configuration. Used by <see cref="SceneRegistry"/>.
    /// </summary>
    public class SceneEntry
    {
        [SerializeField] private string _sceneID;
        [SerializeField] private AssetReference _sceneReference;
        [SerializeField] private string _installerTypeName;
        [SerializeField] private bool _isScoped;
        [SerializeField] private bool _skipContainerInstallation;
        
        /// <summary>
        /// The unique identifier for this scene.
        /// </summary>
        public string SceneID => _sceneID;

        /// <summary>
        /// Addressable asset reference for loading the scene.
        /// </summary>
        public AssetReference SceneReference => _sceneReference;
        
        /// <summary>
        /// Fully qualified type name of the installer class.
        /// Resolved via <see cref="Activator.CreateInstance"/> at load time.
        /// Must have a parameterless constructor.
        /// </summary>
        public string InstallerTypeName => _installerTypeName;
        
        /// <summary>
        /// True if the installer implements IScopedInstaller (receives IScopeBuilder with Import support);
        /// false if it implements IInstaller (receives IContainerBuilder).
        /// </summary>
        public bool IsScoped => _isScoped;
        
        /// <summary>
        /// True to skip DI container installation for this scene
        /// (e.g., persistent transition scene, splash screen).
        /// </summary>
        public bool SkipContainerInstallation => _skipContainerInstallation;

        /// <summary>
        /// Returns the Addressable address string for loading.
        /// Falls back to <see cref="SceneID"/> if no AssetReference is configured.
        /// </summary>
        /// <returns></returns>
        public string GetAddress()
        {
            if(_sceneReference != null && _sceneReference.RuntimeKeyIsValid())
                return _sceneReference.RuntimeKey.ToString();

            return _sceneID;
        }

        internal SceneEntry(
            string sceneID,
            string installerTypeName,
            bool isScoped,
            bool skipContainerInstallation
        )
        {
            _sceneID = sceneID;
            _installerTypeName = installerTypeName;
            _isScoped = isScoped;
            _skipContainerInstallation = skipContainerInstallation;
        }
    }
}