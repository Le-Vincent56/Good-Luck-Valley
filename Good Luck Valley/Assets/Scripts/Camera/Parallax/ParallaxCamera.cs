using UnityEngine;
using UnityEngine.Events;

namespace GoodLuckValley.Cameras.Parallax
{
    [ExecuteInEditMode]
    public class ParallaxCamera : MonoBehaviour
    {
        public UnityAction<float> OnCameraTranslate = delegate { };

        private float prevPosition;

        private void Start()
        {
            // Set the previous position
            prevPosition = transform.position.x;
        }

        private void Update()
        {
            // Check if the position has updated
            if(transform.position.x != prevPosition)
            {
                // Check if OnCameraTranslate is initialized
                if(OnCameraTranslate != null)
                {
                    // Calculate the delta and invoke the action
                    float delta = prevPosition - transform.position.x;
                    OnCameraTranslate.Invoke(delta);
                }

                // Set the previous position
                prevPosition = transform.position.x;
            }
        }
    }
}