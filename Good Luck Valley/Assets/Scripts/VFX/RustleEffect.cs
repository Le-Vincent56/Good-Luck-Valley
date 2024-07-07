using GoodLuckValley.Entities;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class RustleEffect : MonoBehaviour
    {
        // References
        private AreaCollider areaCollider;
        private SpriteRenderer spriteRenderer;

        // Fields
        [SerializeField] private Material windMaterial;
        [SerializeField] private Material rustleMaterial;
        [SerializeField] private bool withWind;
        [SerializeField] private float rustleStrength;
        [SerializeField] private float verticalOffset;
        [SerializeField] private float rustlePercent;
        [SerializeField] private float rustleSpeedScalar;
        private int direction;



        private void Awake()
        {
            // Set references
            areaCollider = GetComponent<AreaCollider>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += Rustle;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= Rustle;
        }

        private void Rustle(GameObject collidedObject)
        {
            // Check if this object has wind, if it does swap to the rustle material
            if (withWind)
                spriteRenderer.material = rustleMaterial;

            // Initialize values
            InitializeShader(collidedObject);

            // Try to stop the coroutine if already rustling
            try
            {
                StopCoroutine(RustleOut());
            }
            catch
            {
                Debug.LogError("Coroutine failed to stop");
            }

            // Start the coroutine
            StartCoroutine(RustleOut());
        }

        private IEnumerator RustleOut()
        {
            // Loop while rustle percent isn't 100% (finished)
            while (rustlePercent < 1)
            {
                // Increase rustle percent by time
                rustlePercent += Mathf.Lerp(0,1, (Time.deltaTime * rustleSpeedScalar));


                // Update rustle percent in the shader
                UpdateFloat("_RustlePercent", rustlePercent);
                yield return null;
            }

            // After coroutine finishes
            StartCoroutine(RustleIn());

        }

        private IEnumerator RustleIn()
        {
            // Loop while rustle percent isn't 100% (finished)
            while (rustlePercent > 0)
            {
                // Increase rustle percent by time
                rustlePercent -= Mathf.Lerp(0, 1, (Time.deltaTime * rustleSpeedScalar));

                // Update rustle percent in the shader
                UpdateFloat("_RustlePercent", rustlePercent);
                yield return null;
            }

            // After coroutine finishes

            // If this object has wind, swap the material back
            if (withWind)
                spriteRenderer.material = windMaterial;

        }

        /// <summary>
        /// Sets initial values for the rustle effect shader
        /// </summary>
        /// <param name="collidedObject">Object detected from collision, to determine direction</param>
        private void InitializeShader(GameObject collidedObject)
        {
            // Check if player is coming from the right or left
            if (collidedObject.transform.position.x > transform.position.x)
            {
                // Coming from right
                direction = -1;
            }
            else
            {
                // From left
                direction = 1;
            }
            // Update shader values
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_RustleStrength", rustleStrength);
            propertyBlock.SetFloat("_xDirection", direction);
            propertyBlock.SetFloat("_VerticalOffset", verticalOffset);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

            // Reset rustle percent
            rustlePercent = 0;
        }
        
        /// <summary>
        /// Updates a float value in the shader
        /// </summary>
        /// <param name="propertyRef">Shader property reference</param>
        /// <param name="propertyValue">Value to set</param>
        private void UpdateFloat(string propertyRef, float propertyValue)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(propertyRef, propertyValue);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }
    }
}