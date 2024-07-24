using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class ShroomRoomLightController : MonoBehaviour
    {
        #region REFERENCE
        [SerializeField] private GameObject prePickupLights;
        [SerializeField] private GameObject postPickupLights;
        [SerializeField] private GameObject mushroomPickupLights;
        [SerializeField] private Light2D volumetricTransitionLight;
        [SerializeField] private VisualEffect pickupVFX;
        #endregion

        #region FIELDS
        [Header("Light Explosion Controls")]
        [SerializeField] private float transitionTime;
        [SerializeField] private float maxLightIntensity;
        [SerializeField] private float lightIntensityIncreaseSpeed;
        [SerializeField] private float lightScaleIncreaseSpeed;
        [SerializeField] private float pickupLightRotationSpeed;
        [SerializeField] private float delayTimeBeforeLightExplosion;
        private float currentTransitionTime;

        [Space(10)]
        [Header("Light Fade Out Controls")]

        [SerializeField] private float deTransitionTime;
        [SerializeField] private float lightIntensityDecreaseSpeed;
        [SerializeField] private float lightScaleDecreaseSpeed;
        #endregion

        private void Update()
        {
            mushroomPickupLights.transform.Rotate(new Vector3(0, 0, Time.deltaTime * pickupLightRotationSpeed), Space.Self);
        }

        public void StartLightTransition()
        {
            pickupVFX.Play();
            StartCoroutine(LightTransition());
        }

        private void SwapLights() 
        {
            Debug.Log("Swap Lights");
            prePickupLights.SetActive(false);
            postPickupLights.SetActive(true);
            pickupVFX.Stop();
        }

        private IEnumerator LightTransition()
        {
            currentTransitionTime = 0;

            yield return new WaitForSeconds(delayTimeBeforeLightExplosion);

            while (currentTransitionTime < transitionTime)
            {
                currentTransitionTime += Time.deltaTime;
                volumetricTransitionLight.intensity += Mathf.Lerp(0, maxLightIntensity, Time.deltaTime * lightIntensityIncreaseSpeed);
                float xScale =  volumetricTransitionLight.gameObject.transform.localScale.x;
                float yScale = volumetricTransitionLight.gameObject.transform.localScale.y;
                volumetricTransitionLight.gameObject.transform.localScale = 
                    new Vector3(xScale += (Time.deltaTime * lightScaleIncreaseSpeed),
                                yScale += (Time.deltaTime * lightScaleIncreaseSpeed),
                                0f);

                yield return null;
            }

            SwapLights();

            StartCoroutine(FadeOutPickupLights());
        }

        private IEnumerator FadeOutPickupLights()
        {
            currentTransitionTime = 0;
            while (currentTransitionTime < deTransitionTime)
            {
                currentTransitionTime += Time.deltaTime;
                volumetricTransitionLight.intensity -= Mathf.Lerp(0, maxLightIntensity, Time.deltaTime * lightIntensityDecreaseSpeed);
                float xScale = volumetricTransitionLight.gameObject.transform.localScale.x;
                float yScale = volumetricTransitionLight.gameObject.transform.localScale.y;
                volumetricTransitionLight.gameObject.transform.localScale =
                    new Vector3(xScale -= (Time.deltaTime * lightScaleDecreaseSpeed),
                                yScale -= (Time.deltaTime * lightScaleDecreaseSpeed),
                                0f);
                yield return null;
            }
        }
    }
}