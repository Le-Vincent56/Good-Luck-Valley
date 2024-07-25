using UnityEngine;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        public float parallaxFactor;

        /// <summary>
        /// Move the parallax layer
        /// </summary>
        /// <param name="delta">The base amount to move</param>
        public void Move(float delta)
        {
            // Get the local position
            Vector3 newPos = transform.localPosition;

            // Change the x-value based on the given delta and parallax factor
            newPos.x -= delta * parallaxFactor * Time.deltaTime;

            // Set the local transform
            transform.localPosition = newPos;
        }
    }
}
