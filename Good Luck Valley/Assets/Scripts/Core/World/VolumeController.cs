using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using HiveMind.Events;
using HiveMind.SaveData;

namespace HiveMind.Core
{
    public class VolumeController : MonoBehaviour, IData
    {
        #region FIELDS
        [SerializeField] private PostProcessingScriptableObj postProcessingEvent;
        [SerializeField] private VolumeProfile volumeProfile;
        [SerializeField] private VolumeProfile loadedProfile;
        [SerializeField] private VolumeProfile baseProfile;

        #region VIGNETTE
        [Header("Vignette")]
        [SerializeField] private Vignette vignette;
        [SerializeField] private Vignette baseVignette;
        [SerializeField] private Color currentColor;
        [SerializeField] private Vector2 currentCenter;
        [Range(01f, 1f)]
        [SerializeField] private float currentIntensity;
        [Range(0f, 1f)]
        [SerializeField] private float currentSmoothness;
        [SerializeField] private bool currentRounded;
        #endregion
        #endregion

        private void OnEnable()
        {
            postProcessingEvent.changeVignette.AddListener(ChangeVignette);
            postProcessingEvent.resetVignette.AddListener(ResetVignette);
        }

        private void OnDisable()
        {
            postProcessingEvent.changeVignette.RemoveListener(ChangeVignette);
            postProcessingEvent.resetVignette.RemoveListener(ResetVignette);
        }

        private void Start()
        {
            // Set defaults
            if(baseProfile.TryGet(out baseVignette))
            {
                postProcessingEvent.SetVignetteDefaultColor(baseVignette.color.value);
                postProcessingEvent.SetVignetteDefaultCenter(baseVignette.center.value);
                postProcessingEvent.SetVignetteDefaultIntensity(baseVignette.intensity.value);
                postProcessingEvent.SetVignetteDefaultSmoothness(baseVignette.smoothness.value);
                postProcessingEvent.SetVignetteDefaultRounded(baseVignette.rounded.value);
            }

            if (loadedProfile != null)
            {
                // Set the volume profile to the loaded profile
                volumeProfile = loadedProfile;

                // Update the current vignette
                if (volumeProfile.TryGet(out vignette))
                {
                    currentColor = vignette.color.value;
                    currentCenter = vignette.center.value;
                    currentIntensity = vignette.intensity.value;
                    currentSmoothness = vignette.smoothness.value;
                    currentRounded = vignette.rounded.value;
                }
            } else
            {
                // Retrieve the vignette
                if(volumeProfile.TryGet(out vignette))
                {
                    // Reset the current vignette
                    ResetVignette();
                }
            }

            // Set the event variables
            postProcessingEvent.SetVignetteColor(currentColor);
            postProcessingEvent.SetVignetteCenter(currentCenter);
            postProcessingEvent.SetVignetteIntensity(currentIntensity);
            postProcessingEvent.SetVignetteSmoothness(currentSmoothness);
            postProcessingEvent.SetVignetteRounded(currentRounded);
        }

        /// <summary>
        /// Change the vignette values of the volume profile
        /// </summary>
        /// <param name="color">The color of the vignette</param>
        /// <param name="targetPosition">The center of the vignette</param>
        /// <param name="intensity">The intensity of the vignette</param>
        public void ChangeVignette(Color color, Vector2 targetPosition, float intensity, bool rounded)
        {
            if (vignette != null)
            {
                // Set color
                vignette.color.value = color;
                currentColor = vignette.color.value;

                // Center the vignette around the target position
                Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(targetPosition);
                Vector2 normalizedScreenPos = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
                vignette.center.value = normalizedScreenPos;
                currentCenter = vignette.center.value;

                // Set intensity
                vignette.intensity.value = Mathf.Clamp(intensity, 0.00f, 1.00f);
                currentIntensity = vignette.intensity.value;

                // Set smoothness
                currentSmoothness = postProcessingEvent.GetVignetteSmoothness();

                // Set rounded
                vignette.rounded.value = rounded;
                currentRounded = vignette.rounded.value;
            }
        }

        /// <summary>
        /// Reset the vignette values of the volume profile
        /// </summary>
        public void ResetVignette()
        {
            // Reset color
            vignette.color.value = baseVignette.color.value;
            currentColor = vignette.color.value;
            postProcessingEvent.SetVignetteColor(currentColor);

            // Reset center
            vignette.center.value = baseVignette.center.value;
            currentCenter = vignette.center.value;
            postProcessingEvent.SetVignetteCenter(currentCenter);

            // Reset intensity
            vignette.intensity.value = baseVignette.intensity.value;
            currentIntensity = vignette.intensity.value;
            postProcessingEvent.SetVignetteIntensity(currentIntensity);

            // Reset smoothness
            vignette.smoothness.value = baseVignette.smoothness.value;
            currentSmoothness = vignette.smoothness.value;
            postProcessingEvent.SetVignetteSmoothness(currentSmoothness);

            // Reset rounded
            vignette.rounded.value = baseVignette.rounded.value;
            currentRounded = vignette.rounded.value;
            postProcessingEvent.SetVignetteRounded(currentRounded);
        }

        #region DATA HANDLING
        public void LoadData(GameData data)
        {
            if(data.volumeProfile != null)
            {
                loadedProfile = data.volumeProfile;
            }
        }

        public void SaveData(GameData data)
        {
            data.volumeProfile = Instantiate(volumeProfile);
        }
        #endregion
    }
}
