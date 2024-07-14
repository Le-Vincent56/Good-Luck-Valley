using GoodLuckValley.World.AreaTriggers;
using System.Collections;
using UnityEngine;

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
        [SerializeField] private float rustleOutSpeedScalar;
        [SerializeField] private float rustleInSpeedScalar;
        private int direction;


        // Swing back and forth after rustled (vines)
        //[SerializeField] private bool continueRustle;
        //private float continueRustleMaxPercent;
        //private bool continueRustleActive;


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

            //if (continueRustle)
            //{
            //    continueRustleMaxPercent = 1;
            //}

            // Try to stop the coroutine if already rustling
            try
            {
                StopCoroutine(RustleOut(1));
            }
            catch
            {
                Debug.LogError("Coroutine failed to stop");
            }

            // Start the coroutine
            StartCoroutine(RustleOut(1));
        }

        private IEnumerator RustleOut(float maxRustlePercent)
        {
            float speedScalar = rustleOutSpeedScalar;
            //if (continueRustleActive)
            //{
            //    speedScalar = rustleInSpeedScalar;
            //}
            // Loop while rustle percent isn't 100% (finished)
            while (rustlePercent < maxRustlePercent)
            {
                UpdateRustlePercent(1, speedScalar, maxRustlePercent);
                yield return null;
            }

            // After coroutine finishes
            StartCoroutine(RustleIn(1));

        }

        private IEnumerator RustleIn(float maxRustlePercent)
        {
            // Loop while rustle percent isn't 100% (finished)
            while (rustlePercent > 0)
            {
                UpdateRustlePercent(-1, rustleInSpeedScalar, maxRustlePercent);
                yield return null;
            }

            // After coroutine finishes

            //if (continueRustle && continueRustleMaxPercent > 0)
            //{
            //    ContinueRustle();
            //}
            //else
            //{
            //    continueRustleActive = false;
            //    
            //}
            if (withWind)
                    spriteRenderer.material = windMaterial;

        }

        //private void ContinueRustle()
        //{
        //    continueRustleActive = true;

        //    // Flip direction
        //    direction *= -1;
        //    UpdateShaderFloatProperty("_xDirection", direction);

        //    continueRustleMaxPercent -= 0.25f;

        //    // Start the coroutine
        //    StartCoroutine(RustleOut(Mathf.Abs(continueRustleMaxPercent)));
        //}

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

            // If this is a vine, it's vertical offset will be negative
            if (verticalOffset < 0)
            {
                // Need to flip direction to compensate
                direction *= -1;
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

        #region HELPERS

        /// <summary>
        /// Updates a float property within the shader
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="propertyValue"></param>
        private void UpdateShaderFloatProperty(string propertyID, float propertyValue)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(propertyID, propertyValue);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }

        /// <summary>
        /// Updates rustle percent based on a given sign (to either subtract or add) 
        /// </summary>
        /// <param name="sign"> The sign that determines if we need to add or subtract from the rustle percent</param>
        private void UpdateRustlePercent(float sign, float speedScalar, float maxRustlePercent)
        {
            // Increase rustle percent by time
            rustlePercent += (Mathf.Lerp(0, maxRustlePercent, (Time.deltaTime * speedScalar)) * sign);

            // Update rustle percent in the shader
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_RustlePercent", rustlePercent);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }
        #endregion
    }
}