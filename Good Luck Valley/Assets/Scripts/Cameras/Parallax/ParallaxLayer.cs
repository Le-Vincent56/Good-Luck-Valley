using GoodLuckValley.Architecture.ServiceLocator;
using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    public class ParallaxLayer : MonoBehaviour
    {
        private Vector3 startPos;
        [SerializeField] float multiplier = 0.0f;
        [SerializeField] bool horizontalOnly = true;
        [SerializeField] private bool initialized = false;

        public Vector3 StartPosition { get => startPos; }
        public float Multiplier { get => multiplier; }
        public bool HorizontalOnly { get => horizontalOnly; }

        /// <summary>
        /// Initialize the Parallax Layer
        /// </summary>
        public void Initialize()
        {
            // Exit case - already initialized
            if (initialized) return;

            startPos = transform.position;
            initialized = true;
        }
    }
}
