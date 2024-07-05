using Cinemachine;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class Parallax : MonoBehaviour
    {
        [Header("Fields")]
        [SerializeField] private float smoothing;
        [SerializeField] private Transform[] layers;
        private float[] parallaxScales;
        private Transform cam;
        private Vector3 previousCamPos;

        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void Start()
        {
            previousCamPos = cam.position;

            parallaxScales = new float[layers.Length];
            for(int i = 0; i < layers.Length; i++)
            {
                parallaxScales[i] = layers[i].position.z * -1;
            }
        }

        private void LateUpdate()
        {
            for(int i = 0; i < layers.Length; i++)
            {
                float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];

                float targetPosX = layers[i].position.x + parallaxX;

                Vector3 targetPos = new Vector3(targetPosX, layers[i].position.y, layers[i].position.z);
                layers[i].position = Vector3.Lerp(layers[i].position, targetPos, smoothing * Time.deltaTime);
            }

            previousCamPos = cam.position;
        }
    }
}