using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX
{
    public class ShroomRoomLightController : MonoBehaviour
    {
        #region REFERENCE
        [SerializeField] private GameObject prePickupLights;
        [SerializeField] private GameObject postPickupLights;
        [SerializeField] private GameObject mushroomPickupLights;
        [SerializeField] private Light2D volumetricTransitionLight;
        #endregion

        #region FIELDS
        [SerializeField] private float transitionTime;
        [SerializeField] private float maxLightIntensity;
        [SerializeField] private float maxLightRadius;
        [SerializeField] private float lightIntensityIncreaseSpeed;
        [SerializeField] private float lightRadiusIncreaseSpeed;
        [SerializeField] private float pickupLightRotationSpeed;
        private float currentTransitionTime;
        #endregion

        private void Update()
        {
            mushroomPickupLights.transform.Rotate(new Vector3(0, 0, Time.deltaTime * pickupLightRotationSpeed), Space.Self);
        }

        public void StartLightTransition()
        {
            StartCoroutine(LightTransition());
        }

        private void SwapLights() 
        {
            Debug.Log("Swap Lights");
            prePickupLights.SetActive(false);
            mushroomPickupLights.SetActive(false);
            postPickupLights.SetActive(true);
        }

        private IEnumerator LightTransition()
        {
            while (currentTransitionTime < transitionTime)
            {
                currentTransitionTime += Time.deltaTime;
                volumetricTransitionLight.intensity += Mathf.Lerp(0, maxLightIntensity, Time.deltaTime * lightIntensityIncreaseSpeed);
                volumetricTransitionLight.pointLightOuterRadius += Mathf.Lerp(0, maxLightRadius, Time.deltaTime * lightRadiusIncreaseSpeed);

                yield return null;
            }

            SwapLights();
        }
    }
}