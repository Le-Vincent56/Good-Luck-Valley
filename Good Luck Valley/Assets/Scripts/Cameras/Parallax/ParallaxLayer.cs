using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    public class ParallaxLayer : MonoBehaviour
    {
        private Vector3 startPos;
        [SerializeField] float multiplier = 0.0f;
        [SerializeField] bool horizontalOnly = true;

        public Vector3 StartPosition { get => startPos; }
        public float Multiplier { get => multiplier; }
        public bool HorizontalOnly { get => horizontalOnly; }

        /// <summary>
        /// Initialize the Parallax Layer
        /// </summary>
        public void Initialize()
        {
            startPos = transform.position;
        }
    }
}
