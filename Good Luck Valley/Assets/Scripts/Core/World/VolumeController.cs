using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
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
        [SerializeField] private Color currentVignetteColor;
        [SerializeField] private Vector2 currentVignetteCenter;
        [Range(01f, 1f)]
        [SerializeField] private float currentVignetteIntensity;
        [Range(0f, 1f)]
        [SerializeField] private float currentVignetteSmoothness;
        [SerializeField] private bool currentVignetteRounded;
        #endregion

        #region CHROMATIC ABBERATION
        [SerializeField] private ChromaticAberration chromaticAberration;
        [SerializeField] private ChromaticAberration baseChromaticAberration;
        [Range(01f, 1f)]
        [SerializeField] private float currentAberrationIntensity;
        #endregion

        #region COLOR CURVES
        [SerializeField] private ColorCurves colorCurves;
        [SerializeField] private ColorCurves baseColorCurves;
        [SerializeField] private TextureCurve currentTextureCurve;
        #endregion

        #region FILM GRAIN
        [SerializeField] private FilmGrain filmGrain;
        [SerializeField] private FilmGrain baseFilmGrain;
        [Range(01f, 1f)]
        [SerializeField] private float currentFilmGrainIntensity;
        [Range(01f, 1f)]
        [SerializeField] private float currentFilmGrainResponse;
        #endregion
        #endregion

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            postProcessingEvent.changeVignette.AddListener(ChangeVignette);
            postProcessingEvent.changeAberration.AddListener(ChangeAberration);
            postProcessingEvent.changeColorCurves.AddListener(ChangeColorCurves);
            postProcessingEvent.changeFilmGrain.AddListener(ChangeFilmGrain);
            postProcessingEvent.resetVignette.AddListener(ResetVignette);
            postProcessingEvent.resetAberration.AddListener(ResetAberration);
            postProcessingEvent.resetColorCurves.AddListener(ResetColorCurves);
            postProcessingEvent.resetFilmGrain.AddListener(ResetFilmGrain);
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            postProcessingEvent.changeVignette.RemoveListener(ChangeVignette);
            postProcessingEvent.changeAberration.RemoveListener(ChangeAberration);
            postProcessingEvent.changeColorCurves.RemoveListener(ChangeColorCurves);
            postProcessingEvent.changeFilmGrain.RemoveListener(ChangeFilmGrain);
            postProcessingEvent.resetVignette.RemoveListener(ResetVignette);
            postProcessingEvent.resetAberration.RemoveListener(ResetAberration);
            postProcessingEvent.resetColorCurves.RemoveListener(ResetColorCurves);
            postProcessingEvent.resetFilmGrain.RemoveListener(ResetFilmGrain);
        }

        private void OnSceneUnloaded(Scene mode)
        {
            // Reset all on unload so the Scene view isn't altered
            ResetVignette();
            ResetAberration();
            ResetColorCurves();
            ResetFilmGrain();
        }

        private void Start()
        {
            #region VIGNETTE
            // Set defaults
            if (baseProfile.TryGet(out baseVignette))
            {
                postProcessingEvent.SetVignetteDefaultColor(baseVignette.color.value);
                postProcessingEvent.SetVignetteDefaultCenter(baseVignette.center.value);
                postProcessingEvent.SetVignetteDefaultIntensity(baseVignette.intensity.value);
                postProcessingEvent.SetVignetteDefaultSmoothness(baseVignette.smoothness.value);
                postProcessingEvent.SetVignetteDefaultRounded(baseVignette.rounded.value);
            }

            // Check if a profile has been loaded
            if (loadedProfile != null)
            {
                // Set the volume profile to the loaded profile
                volumeProfile = loadedProfile;

                // Set the last saved values
                if (volumeProfile.TryGet(out vignette))
                {
                    currentVignetteCenter = vignette.center.value;
                    currentVignetteIntensity = vignette.intensity.value;
                    currentVignetteSmoothness = vignette.smoothness.value;
                    currentVignetteRounded = vignette.rounded.value;
                    currentVignetteColor = vignette.color.value;
                }
            } else
            {
                // Otherwise, reset values
                if(volumeProfile.TryGet(out vignette))
                {
                    ResetVignette();
                }
            }

            // Set the event variables
            postProcessingEvent.SetVignetteColor(currentVignetteColor);
            postProcessingEvent.SetVignetteCenter(currentVignetteCenter);
            postProcessingEvent.SetVignetteIntensity(currentVignetteIntensity);
            postProcessingEvent.SetVignetteSmoothness(currentVignetteSmoothness);
            postProcessingEvent.SetVignetteRounded(currentVignetteRounded);
            #endregion

            #region CHROMATIC ABBERATION
            // Set defaults
            if(baseProfile.TryGet(out baseChromaticAberration))
            {
                postProcessingEvent.SetAberrationDefaultIntensity(baseChromaticAberration.intensity.value);
            }

            // Check if a profile has been loaded
            if(loadedProfile != null)
            {
                // If so, set the last saved values
                if(volumeProfile.TryGet(out chromaticAberration))
                {
                    currentAberrationIntensity = chromaticAberration.intensity.value;
                }
            } else
            {
                // Otherwise, reset values
                if(volumeProfile.TryGet(out chromaticAberration))
                {
                    ResetAberration();
                }
            }

            // Set the event variables
            postProcessingEvent.SetAberrationIntensity(currentAberrationIntensity);
            #endregion

            #region COLOR CURVES
            // Set defaults
            if(baseProfile.TryGet(out baseColorCurves))
            {
                postProcessingEvent.SetDefaultColorCurve(baseColorCurves.hueVsSat.value);
            }

            // Check if a profile has been loaded
            if (loadedProfile != null)
            {
                // If so, set the last saved values
                if(volumeProfile.TryGet(out colorCurves))
                {
                    currentTextureCurve = colorCurves.hueVsSat.value;
                }
            } else
            {
                // Otherwise, reset values
                if(volumeProfile.TryGet(out colorCurves))
                {
                    ResetColorCurves();
                }
            }

            // Set the event variables
            postProcessingEvent.SetColorCurve(currentTextureCurve);
            #endregion

            #region FILM GRAIN
            // Set defaults
            if(baseProfile.TryGet(out baseFilmGrain))
            {
                postProcessingEvent.SetDefaultFilmGrainIntensity(baseFilmGrain.intensity.value);
                postProcessingEvent.SetDefaultFilmGrainResponse(baseFilmGrain.response.value);
            }

            // Check if a profile has been loaded
            if(loadedProfile != null)
            {
                if(volumeProfile.TryGet(out filmGrain))
                {
                    currentFilmGrainIntensity = filmGrain.intensity.value;
                    currentFilmGrainResponse = filmGrain.response.value;
                }
            } else
            {
                if(volumeProfile.TryGet(out filmGrain))
                {
                    ResetFilmGrain();
                }
            }

            // Set the event variables
            postProcessingEvent.SetFilmGrainIntensity(currentFilmGrainIntensity);
            postProcessingEvent.SetFilmGrainResponse(currentFilmGrainResponse);
            #endregion
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
                currentVignetteColor = vignette.color.value;

                // Center the vignette around the target position
                Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(targetPosition);
                Vector2 normalizedScreenPos = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
                vignette.center.value = normalizedScreenPos;
                currentVignetteCenter = vignette.center.value;

                // Set intensity
                vignette.intensity.value = Mathf.Clamp(intensity, 0.00f, 1.00f);
                currentVignetteIntensity = vignette.intensity.value;

                // Set smoothness
                vignette.smoothness.value = postProcessingEvent.GetVignetteSmoothness();
                currentVignetteSmoothness = vignette.smoothness.value;

                // Set rounded
                vignette.rounded.value = rounded;
                currentVignetteRounded = vignette.rounded.value;
            }
        }

        /// <summary>
        /// Change the aberration values of the volume profile
        /// </summary>
        /// <param name="intensity">The intensity of the aberration</param>
        public void ChangeAberration(float intensity)
        {
            // Set intensity
            chromaticAberration.intensity.value = intensity;
            currentAberrationIntensity = chromaticAberration.intensity.value;
            postProcessingEvent.SetAberrationIntensity(currentAberrationIntensity);
        }

        /// <summary>
        /// Change the color curves of the volume profile
        /// </summary>
        /// <param name="colorCurve">The color curve</param>
        public void ChangeColorCurves(TextureCurve colorCurve)
        {
            // Set the color curve
            colorCurves.hueVsSat.value = colorCurve;
            currentTextureCurve = colorCurves.hueVsSat.value;
            postProcessingEvent.SetColorCurve(currentTextureCurve);
        }

        public void ChangeFilmGrain(float intensity, float response)
        {
            Debug.LogWarning("Changing film grain");
            // Set intensity
            filmGrain.intensity.value = intensity;
            currentFilmGrainIntensity = filmGrain.intensity.value;
            postProcessingEvent.SetFilmGrainIntensity(filmGrain.intensity.value);

            // Set response
            filmGrain.response.value = response;
            currentFilmGrainResponse = filmGrain.response.value;
            postProcessingEvent.SetFilmGrainResponse(filmGrain.response.value);
        }

        /// <summary>
        /// Reset the vignette values of the volume profile
        /// </summary>
        public void ResetVignette()
        {
            // Reset color
            vignette.color.value = baseVignette.color.value;
            currentVignetteColor = vignette.color.value;
            postProcessingEvent.SetVignetteColor(currentVignetteColor);

            // Reset center
            vignette.center.value = baseVignette.center.value;
            currentVignetteCenter = vignette.center.value;
            postProcessingEvent.SetVignetteCenter(currentVignetteCenter);

            // Reset intensity
            vignette.intensity.value = baseVignette.intensity.value;
            currentVignetteIntensity = vignette.intensity.value;
            postProcessingEvent.SetVignetteIntensity(currentVignetteIntensity);

            // Reset smoothness
            vignette.smoothness.value = baseVignette.smoothness.value;
            currentVignetteSmoothness = vignette.smoothness.value;
            postProcessingEvent.SetVignetteSmoothness(currentVignetteSmoothness);

            // Reset rounded
            vignette.rounded.value = baseVignette.rounded.value;
            currentVignetteRounded = vignette.rounded.value;
            postProcessingEvent.SetVignetteRounded(currentVignetteRounded);
        }

        /// <summary>
        /// Reset the chromatic abberation values of the volume profile
        /// </summary>
        public void ResetAberration()
        {
            // Reset intensity
            chromaticAberration.intensity.value = baseChromaticAberration.intensity.value;
            currentAberrationIntensity = chromaticAberration.intensity.value;
            postProcessingEvent.SetAberrationIntensity(currentAberrationIntensity);
        }

        /// <summary>
        /// Reset the color curves of the volume profile
        /// </summary>
        public void ResetColorCurves()
        {
            // Reset the TextureCurve
            colorCurves.hueVsSat.value = baseColorCurves.hueVsSat.value;
            currentTextureCurve = colorCurves.hueVsSat.value;
            postProcessingEvent.SetColorCurve(currentTextureCurve);
        } 

        public void ResetFilmGrain()
        {
            Debug.LogWarning("Resetting film grain");

            // Reset intensity
            filmGrain.intensity.value = baseFilmGrain.intensity.value;
            currentFilmGrainIntensity = filmGrain.intensity.value;
            postProcessingEvent.SetFilmGrainIntensity(currentFilmGrainIntensity);

            // Reset response
            filmGrain.response.value = baseFilmGrain.response.value;
            currentFilmGrainResponse = filmGrain.response.value;
            postProcessingEvent.SetFilmGrainResponse(currentFilmGrainResponse);
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
