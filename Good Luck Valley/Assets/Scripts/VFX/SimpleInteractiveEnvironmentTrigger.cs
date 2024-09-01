using GoodLuckValley.Events;
using GoodLuckValley.World.AreaTriggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{

    [RequireComponent(typeof(AreaCollider))]



    public class SimpleInteractiveEnvironmentTrigger : MonoBehaviour
    {
        public enum PlantType
        {
            Blue,
            Purple
        }

        public struct InteractiveEnvironmentData
        {
            public Vector3 Position;
            public PlantType PlantType;

            public InteractiveEnvironmentData(Vector3 position, PlantType plantType)
            {
                Position = position;
                PlantType = plantType;
            }
        }

        #region FIELDS
        [SerializeField] private bool hasLight;
        [SerializeField] private bool hasParticle;
        [SerializeField] private float currentReactiveTime;
        [SerializeField] private float maxIntensity;
        [SerializeField] private float maxIntensityHoldTime;
        [SerializeField] private float intensityChangeSpeed;
        [SerializeField] private PlantType plantType;
        private bool coroutineActive;
        #endregion

        #region REFERENCES
        [SerializeField] private Light2D reactiveLight;
        [SerializeField] private GameEvent onPlayEnvironmentParticle;
        private AreaCollider areaCollider;
        #endregion

        private void Awake()
        {
            areaCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            areaCollider.OnTriggerEnter += StartReactiveElement;
        }

        private void OnDisable()
        {
            areaCollider.OnTriggerEnter -= StartReactiveElement;
        }

        private void StartReactiveElement(GameObject hitObject)
        {
            Debug.Log("starting: " + coroutineActive);
            if (!coroutineActive)
            {
                coroutineActive = true;
                currentReactiveTime = 0;
                reactiveLight.intensity = 0;

                if (hasLight)
                    StartCoroutine(ReactiveIncrease());

                if (hasParticle)
                    PlayParticle();
            }
        }

        private IEnumerator ReactiveIncrease()
        {
            while (reactiveLight.intensity < maxIntensity)
            {
                currentReactiveTime += Time.deltaTime * intensityChangeSpeed;
                reactiveLight.intensity += Mathf.Lerp(0, maxIntensity, currentReactiveTime);
                yield return null;
            }
            currentReactiveTime = 0;

            yield return new WaitForSeconds(maxIntensityHoldTime);

            StartCoroutine(ReactiveDecrease());
        }

        private IEnumerator ReactiveDecrease()
        {
            while (reactiveLight.intensity > 0)
            {
                currentReactiveTime += Time.deltaTime * intensityChangeSpeed;
                reactiveLight.intensity -= Mathf.Lerp(0, maxIntensity, currentReactiveTime);
                yield return null;
            }

            coroutineActive = false;
        }

        private void PlayParticle()
        {
            InteractiveEnvironmentData interactiveEnvironmentData = new InteractiveEnvironmentData(transform.position, plantType);
            onPlayEnvironmentParticle.Raise(this, interactiveEnvironmentData);
        }
    }
}