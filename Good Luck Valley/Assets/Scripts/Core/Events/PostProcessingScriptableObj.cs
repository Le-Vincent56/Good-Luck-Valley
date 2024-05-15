using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace HiveMind.Events
{
    [CreateAssetMenu(fileName = "PostProcessingScriptableObject", menuName = "ScriptableObjects/Post-Processing Event")]
    public class PostProcessingScriptableObj : ScriptableObject
    {
        #region FIELDS
        #region VIGNETTE
        [SerializeField] private Color vignetteDefaultColor;
        [SerializeField] private Color vignetteColor;
        [SerializeField] private Vector2 vignetteDefaultCenter;
        [SerializeField] private Vector2 vignetteCenter;
        [SerializeField] private float vignetteDefaultIntensity;
        [SerializeField] private float vignetteIntensity;
        [SerializeField] private float vignetteDefaultSmoothness;
        [SerializeField] private float vignetteSmoothness;
        [SerializeField] private bool vignetteDefaultRounded;
        [SerializeField] private bool vignetteRounded;
        #endregion

        #region CHROMATIC ABERRATION
        [SerializeField] private float aberrationDefaultIntensity;
        [SerializeField] private float aberrationIntensity;
        #endregion

        #region COLOR CURVES
        [SerializeField] private TextureCurve defaultCurve;
        [SerializeField] private TextureCurve curve;
        #endregion

        #region FILM GRAIN
        [SerializeField] private float defaultFilmGrainIntensity;
        [SerializeField] private float filmGrainIntensity;
        [SerializeField] private float defaultFilmGrainResponse;
        [SerializeField] private float filmGrainResponse;
        #endregion

        #region EVENTS
        public UnityEvent<Color, Vector2, float, bool> changeVignette;
        public UnityEvent<float> changeAberration;
        public UnityEvent<TextureCurve> changeColorCurves;
        public UnityEvent<float, float> changeFilmGrain;
        public UnityEvent resetVignette;
        public UnityEvent resetAberration;
        public UnityEvent resetColorCurves;
        public UnityEvent resetFilmGrain;
        #endregion
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (changeVignette == null)
            {
                changeVignette = new UnityEvent<Color, Vector2, float, bool>();
            }

            if (changeAberration == null)
            {
                changeAberration = new UnityEvent<float>();
            }

            if(changeColorCurves == null)
            {
                changeColorCurves = new UnityEvent<TextureCurve>();
            }

            if(changeFilmGrain == null)
            {
                changeFilmGrain = new UnityEvent<float, float>();
            }

            if (resetVignette == null)
            {
                resetVignette = new UnityEvent();
            }

            if(resetAberration == null)
            {
                resetAberration = new UnityEvent();
            }

            if(resetColorCurves == null)
            {
                resetColorCurves = new UnityEvent();
            }

            if(resetFilmGrain == null)
            {
                resetFilmGrain = new UnityEvent();
            }
            #endregion
        }

        #region SET - VIGNETTE
        /// <summary>
        /// Set the default color of the vignette
        /// </summary>
        /// <param name="vignetteDefaultColor">The default color of the vignette</param>
        public void SetVignetteDefaultColor(Color vignetteDefaultColor)
        {
            this.vignetteDefaultColor = vignetteDefaultColor;
        }

        /// <summary>
        /// Set the current vignette color
        /// </summary>
        /// <param name="vignetteColor">The vignette color to set</param>
        public void SetVignetteColor(Color vignetteColor)
        {
            this.vignetteColor = vignetteColor;
        }

        /// <summary>
        /// Set the default center of the vignette
        /// </summary>
        /// <param name="vignetteDefaultCenter">The default center of the vignette</param>
        public void SetVignetteDefaultCenter(Vector2 vignetteDefaultCenter)
        {
            this.vignetteDefaultCenter = vignetteDefaultCenter;
        }

        /// <summary>
        /// Set the current center of the vignette
        /// </summary>
        /// <param name="vignetteCenter">The current center of the vignette</param>
        public void SetVignetteCenter(Vector2 vignetteCenter)
        {
            this.vignetteCenter = vignetteCenter;
        }

        /// <summary>
        /// Set the default intensity of the vignette
        /// </summary>
        /// <param name="vignetteDefaultIntensity">The default intensity of the vignette</param>
        public void SetVignetteDefaultIntensity(float vignetteDefaultIntensity)
        {
            this.vignetteDefaultIntensity = vignetteDefaultIntensity;
        }

        /// <summary>
        /// Set the current vignette intensity
        /// </summary>
        /// <param name="vignetteIntensity">The vignette intensity to set</param>
        public void SetVignetteIntensity(float vignetteIntensity)
        {
            this.vignetteIntensity = vignetteIntensity;
        }

        /// <summary>
        /// Set the default smoothness of the vignette
        /// </summary>
        /// <param name="vignetteDefaultSmoothness">The default smoothness of the vignette</param>
        public void SetVignetteDefaultSmoothness(float vignetteDefaultSmoothness)
        {
            this.vignetteDefaultSmoothness = vignetteDefaultSmoothness;
        }

        /// <summary>
        /// Set the current vignette smoothness
        /// </summary>
        /// <param name="vignetteSmoothess">The vignette smoothness to set</param>
        public void SetVignetteSmoothness(float vignetteSmoothness)
        {
            this.vignetteSmoothness = Mathf.Clamp(vignetteSmoothness, 0.0f, 1.0f);
        }

        /// <summary>
        /// Set the default rounded of the vignette
        /// </summary>
        /// <param name="vignetteDefaultRounded">The default rounded of the vignette</param>
        public void SetVignetteDefaultRounded(bool vignetteDefaultRounded)
        {
            this.vignetteDefaultRounded = vignetteDefaultRounded;
        }

        /// <summary>
        /// Set teh current rounded of the vignette
        /// </summary>
        /// <param name="vignetteRounded">The current rounded of the vignette</param>
        public void SetVignetteRounded(bool vignetteRounded)
        {
            this.vignetteRounded = vignetteRounded;
        }
        #endregion

        #region SET - CHROMATIC ABERRATION
        /// <summary>
        /// Set the default intensity of the chromatic aberration
        /// </summary>
        /// <param name="aberrationDefaultIntensity">The default intensity of the chromatic aberration</param>
        public void SetAberrationDefaultIntensity(float aberrationDefaultIntensity)
        {
            this.aberrationDefaultIntensity = aberrationDefaultIntensity;
        }

        /// <summary>
        /// Set the current intensity of the chromatic aberration
        /// </summary>
        /// <param name="aberrationIntensity">The current intensity of the chromatic aberration</param>
        public void SetAberrationIntensity(float aberrationIntensity)
        {
            this.aberrationIntensity = Mathf.Clamp(aberrationIntensity, 0.0f, 1.0f);
        }
        #endregion

        #region SET - COLOR CURVES
        /// <summary>
        /// Set the default curve of the color curve
        /// </summary>
        /// <param name="defaultCurve">The default curve of the color curve</param>
        public void SetDefaultColorCurve(TextureCurve defaultCurve)
        {
            this.defaultCurve = defaultCurve;
        }

        /// <summary>
        /// Set the current curve of the color curve
        /// </summary>
        /// <param name="curve">he current curve of the color curve</param>
        public void SetColorCurve(TextureCurve curve)
        {
            this.curve = curve;
        }

        #region SET - FILM GRAIN
        /// <summary>
        /// Set the default intensity fo the film grain
        /// </summary>
        /// <param name="defaultFilmGrainIntensity">The default intensity fo the film grain</param>
        public void SetDefaultFilmGrainIntensity(float defaultFilmGrainIntensity)
        {
            this.defaultFilmGrainIntensity = defaultFilmGrainIntensity;
        }

        /// <summary>
        /// Set the current intensity of the film grain
        /// </summary>
        /// <param name="filmGrainIntensity">The current intensity of the film grain</param>
        public void SetFilmGrainIntensity(float filmGrainIntensity)
        {
            this.filmGrainIntensity = filmGrainIntensity;
        }

        /// <summary>
        /// Set the default film grain response
        /// </summary>
        /// <param name="defaultFilmGrainResponse">The default film grain response</param>
        public void SetDefaultFilmGrainResponse(float defaultFilmGrainResponse)
        {
            this.defaultFilmGrainResponse = defaultFilmGrainResponse;
        }

        /// <summary>
        /// Set the current film grain response
        /// </summary>
        /// <param name="filmGrainResponse">The current film grain response</param>
        public void SetFilmGrainResponse(float filmGrainResponse)
        {
            this.filmGrainResponse = filmGrainResponse;
        }
        #endregion
        #endregion

        #region GET - VIGNETTE
        /// <summary>
        /// Get the default color of the vignette
        /// </summary>
        /// <returns>The default color of the vignette</returns>
        public Color GetVignetteDefaultColor()
        {
            return vignetteDefaultColor;
        }

        /// <summary>
        /// Get the current vignette color
        /// </summary>
        /// <returns>The current vignette color</returns>
        public Color GetVignetteColor()
        {
            return vignetteColor;
        }

        /// <summary>
        /// Get the default center of the vignette
        /// </summary>
        /// <returns>The default center of the vignette</returns>
        public Vector2 GetVignetteDefaultCenter()
        {
            return vignetteDefaultCenter;
        }

        /// <summary>
        /// Get the current center of the vignette
        /// </summary>
        /// <returns>The current center of the vignette</returns>
        public Vector2 GetVignetteCenter()
        {
            return vignetteCenter;
        }

        /// <summary>
        /// Get the default intensity of the vignette
        /// </summary>
        /// <returns>The default intensity of the vignette</returns>
        public float GetVignetteDefaultIntensity()
        {
            return vignetteDefaultIntensity;
        }

        /// <summary>
        /// Get the current vignette intensity
        /// </summary>
        /// <returns>The current vignette intensity</returns>
        public float GetVignetteIntensity()
        {
            return vignetteIntensity;
        }

        /// <summary>
        /// Get the default smoothness of the vignette
        /// </summary>
        /// <returns>The default smoothness of the vignette</returns>
        public float GetVignetteDefaultSmoothness()
        {
            return vignetteDefaultSmoothness;
        }

        /// <summary>
        /// Get the current smoothness of the vignette
        /// </summary>
        /// <returns>The current smoothness of the vignette</returns>
        public float GetVignetteSmoothness()
        {
            return vignetteSmoothness;
        }

        /// <summary>
        /// Get the default rounded of the vignette
        /// </summary>
        /// <returns>The default rounded of the vignette</returns>
        public bool GetVignetteDefaultRounded()
        {
            return vignetteDefaultRounded;
        }

        /// <summary>
        /// Get the current rounded of the vignette
        /// </summary>
        /// <returns>The current rounded of the vignette</returns>
        public bool GetVignetteRounded()
        {
            return vignetteRounded;
        }
        #endregion

        #region GET - CHROMATIC ABERRATION
        /// <summary>
        /// Get the default intensity of the chromatic aberration
        /// </summary>
        /// <returns>The default intensity of the chromatic aberration</returns>
        public float GetAberrationDefaultIntensity()
        {
            return aberrationDefaultIntensity;
        }

        /// <summary>
        /// Get the current intensity of the chromatic aberration
        /// </summary>
        /// <returns>The current intensity of the chromatic aberration</returns>
        public float GetAberrationIntensity()
        {
            return aberrationIntensity;
        }
        #endregion

        #region GET - COLOR CURVES
        /// <summary>
        /// Set the default curve of the color curve
        /// </summary>
        /// <returns>The default curve of the color curve</returns>
        public TextureCurve GetDefaultColorCurve()
        {
            return defaultCurve;
        }

        /// <summary>
        /// Set the current curve of the color curve
        /// </summary>
        /// <returns>The current curve of the color curve</returns>
        public TextureCurve GetColorCurve()
        {
            return curve;
        }
        #endregion

        #region GET - FILM GRAIN
        /// <summary>
        /// Get the default intensity of the film grain
        /// </summary>
        /// <returns>The default intensity of the film grain</returns>
        public float GetDefaultFilmGrainIntensity()
        {
            return defaultFilmGrainIntensity;
        }

        /// <summary>
        /// Get the current intensity of the film grain
        /// </summary>
        /// <returns>The current intensity of the film grain</returns>
        public float GetFilmGrainIntensity()
        {
            return filmGrainIntensity;
        }

        /// <summary>
        /// Get the default response of the film grain
        /// </summary>
        /// <returns>The default response of the film grain</returns>
        public float GetDefaultFilmGrainResponse()
        {
            return defaultFilmGrainResponse;
        }

        /// <summary>
        /// Get the current response of the film grain
        /// </summary>
        /// <returns>The current response of the film grain</returns>
        public float GetFilmGrainResponse()
        {
            return filmGrainResponse;
        }
        #endregion

        /// <summary>
        /// Center the vignette around a position
        /// </summary>
        /// <param name="targetPosition">The position of the center</param>
        /// <param name="intensity">The intensity of the vignette</param>
        /// <param name="color">The color of the vignette</param>
        public void ChangeVignette(Color color, Vector2 targetPosition, float intensity, bool rounded)
        {
            changeVignette.Invoke(color, targetPosition, intensity, rounded);
        }

        /// <summary>
        /// Change the chromatic aberration values of the profile
        /// </summary>
        /// <param name="intensity">The intensity to change to</param>
        public void ChangeAberration(float intensity)
        {
            changeAberration.Invoke(intensity);
        }

        /// <summary>
        /// Change the color curve values of the profile
        /// </summary>
        /// <param name="colorCurve">The color curve to change to</param>
        public void ChangeColorCurves(TextureCurve colorCurve)
        {
            changeColorCurves.Invoke(colorCurve);
        }

        /// <summary>
        /// Change the film grain of the profile
        /// </summary>
        /// <param name="intensity">The intensity of the film grain</param>
        /// <param name="response">The response of the film grain</param>
        public void ChangeFilmGrain(float intensity, float response)
        {
            changeFilmGrain.Invoke(intensity, response);
        }

        /// <summary>
        /// Reset the values of the vignette
        /// </summary>
        public void ResetVignette()
        {
            resetVignette.Invoke();
        }

        /// <summary>
        /// Reset the values of the chromatic aberration
        /// </summary>
        public void ResetAberration()
        {
            resetAberration.Invoke();
        }

        /// <summary>
        /// Reset the values of the color curves
        /// </summary>
        public void ResetColorCurve()
        {
            resetColorCurves.Invoke();
        }

        /// <summary>
        /// Reset the values of the film grain
        /// </summary>
        public void ResetFilmGrain()
        {
            resetFilmGrain.Invoke();
        }
    }
}
