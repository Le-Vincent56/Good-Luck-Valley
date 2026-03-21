using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Core.SceneManagement.Adapters
{
    /// <summary>
    /// MonoBehaviour adapter that drives a screen-space transition overlay.
    /// Sits on a persistent Canvas in the transition scene. Manipulates a
    /// RawImage's shader material _Progress property (0 = transparent, 1 = opaque).
    /// Not registered in DI - TransitionService holds a direct reference via
    /// <see cref="Interfaces.ITransitionService.SetCanvasAdapter"/>.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class TransitionCanvasAdapter : MonoBehaviour
    {
        [SerializeField] private RawImage _overlay;
        [SerializeField] private Material _transitionMaterial;

        private static readonly int _progressProperty = Shader.PropertyToID("_Progress");

        private Material _materialInstance;

        public Material MaterialInstance => _materialInstance;

        private void Awake()
        {
            if (_transitionMaterial)
            {
                _materialInstance = new Material(_transitionMaterial);
                _overlay.material = _materialInstance;
            }

            SetOverlayActive(false);
        }
        
        private void OnDestroy()
        {
            if (!_materialInstance) return;
            
            Destroy(_materialInstance);
        }

        /// <summary>
        /// Sets the transition progress on the shader material.
        /// </summary>
        /// <param name="progress">Progress value from 0 (transparent) to 1 (opaque).</param>
        public void SetProgress(float progress)
        {
            if (!_materialInstance) return;
            
            _materialInstance.SetFloat(_progressProperty, progress);
        }

        /// <summary>
        /// Swaps the transition material for a different effect.
        /// Creates a material instance to avoid modifying the shared asset.
        /// </summary>
        /// <param name="material">The new shared material to instance.</param>
        public void SetMaterial(Material material)
        {
            if (_materialInstance) Destroy(_materialInstance);
            
            _materialInstance = new Material(material);
            _overlay.material = _materialInstance;
        }

        /// <summary>
        /// Shows or hides the overlay image.
        /// </summary>
        /// <param name="active">True to show the overlay, false to hide it.</param>
        public void SetOverlayActive(bool active) => _overlay.enabled = active;
    }
}