using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        private Vector3 startPos;
        [SerializeField] protected float multiplier = 0.0f;
        [SerializeField] private bool horizontalOnly = true;
        [SerializeField] private bool initialized = false;

        public Vector3 StartPosition { get => startPos; }
        public float Multiplier { get => multiplier; }
        public bool HorizontalOnly { get => horizontalOnly; }
        public bool Initialized { get => initialized; }

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
