using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GoodLuckValley.Core.SceneManagement.Data
{
    /// <summary>
    /// Serializable entry mapping a scene ID to its Addressable reference
    /// and DI installer configuration. Used by <see cref="SceneRegistry"/>.
    /// </summary>
    [Serializable]
    public class SceneEntry
    {
        [SerializeField] private string sceneID;
        [SerializeField] private AssetReference sceneReference;
        [SerializeField] private string installerTypeName;
        [SerializeField] private bool isScoped;
        [SerializeField] private bool skipContainerInstallation;
        [SerializeField] private int stableID;
        
        /// <summary>
        /// The unique identifier for this scene.
        /// </summary>
        public string SceneID => sceneID;

        /// <summary>
        /// Addressable asset reference for loading the scene.
        /// </summary>
        public AssetReference SceneReference => sceneReference;
        
        /// <summary>
        /// Fully qualified type name of the installer class.
        /// Resolved via <see cref="Activator.CreateInstance"/> at load time.
        /// Must have a parameterless constructor.
        /// </summary>
        public string InstallerTypeName => installerTypeName;
        
        /// <summary>
        /// True if the installer implements IScopedInstaller (receives IScopeBuilder with Import support);
        /// false if it implements IInstaller (receives IContainerBuilder).
        /// </summary>
        public bool IsScoped => isScoped;
        
        /// <summary>
        /// True to skip DI container installation for this scene
        /// (e.g., persistent transition scene, splash screen).
        /// </summary>
        public bool SkipContainerInstallation => skipContainerInstallation;
        
        /// <summary>
        /// Deterministic integer hash of <see cref="SceneID"/>, computed
        /// via <see cref="Utilities.HashUtility.ComputeStableHash"/> at edit time.
        /// </summary>
        public int StableID => stableID;
        
        private SceneEntry() { }

        internal SceneEntry(
            string sceneID,
            string installerTypeName,
            bool isScoped,
            bool skipContainerInstallation,
            int stableID = 0
        )
        {
            this.sceneID = sceneID;
            this.installerTypeName = installerTypeName;
            this.isScoped = isScoped;
            this.skipContainerInstallation = skipContainerInstallation;
            this.stableID = stableID;
        }

        /// <summary>
        /// Returns the Addressable address string for loading.
        /// Falls back to <see cref="SceneID"/> if no AssetReference is configured.
        /// </summary>
        /// <returns></returns>
        public string GetAddress()
        {
            if(sceneReference != null && sceneReference.RuntimeKeyIsValid())
                return sceneReference.RuntimeKey.ToString();

            return sceneID;
        }

        /// <summary>
        /// Sets the stable ID for the scene entry. Used by editor tooling.
        /// </summary>
        /// <param name="id">The stable ID to assign to the scene entry.</param>
        internal void SetStableID(int id) => stableID = id;
    }
}