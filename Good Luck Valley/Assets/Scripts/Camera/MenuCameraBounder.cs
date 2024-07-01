using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class MenuCameraBounder : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onUpdateScreenBounds;
        Bounds cameraBounds;

        // Start is called before the first frame update
        void Start()
        {
            // Calculate the camera bounds
            cameraBounds = CalculateCameraBounds();

            onUpdateScreenBounds.Raise(this, cameraBounds);
        }

        /// <summary>
        /// Calculate the bounds of the camera
        /// </summary>
        /// <returns></returns>
        private Bounds CalculateCameraBounds()
        {
            float height = 2f * Camera.main.orthographicSize;
            float width = height * Camera.main.aspect;

            Vector3 center = Camera.main.transform.position;
            Vector3 size = new Vector3(width, height);

            return new Bounds(center, size);
        }
    }
}