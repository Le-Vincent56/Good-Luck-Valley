using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HiveMind.Events
{
    [CreateAssetMenu(fileName = "PostProcessingScriptableObject", menuName = "ScriptableObjects/Post-Processing Event")]
    public class PostProcessingScriptableObj : ScriptableObject
    {
        #region FIELDS
        [SerializeField] private Color vignetteDefaultColor;
        [SerializeField] private Color vignetteColor;
        [SerializeField] private Vector2 vignetteDefaultCenter;
        [SerializeField] private Vector2 vignetteCenter;
        [SerializeField] private float vignetteDefaultIntensity;
        [SerializeField] private float vignetteIntensity;
        [SerializeField] private float vignetteDefaultSmoothness;
        [SerializeField] private float vignetteSmoothess;
        [SerializeField] private bool vignetteDefaultRounded;
        [SerializeField] private bool vignetteRounded;

        #region EVENTS
        public UnityEvent<Color, Vector2, float, bool> changeVignette;
        public UnityEvent resetVignette;
        #endregion
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if (changeVignette == null)
            {
                changeVignette = new UnityEvent<Color, Vector2, float, bool>();
            }

            if(resetVignette == null)
            {
                resetVignette = new UnityEvent();
            }
            #endregion
        }

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
        public void SetVignetteSmoothness(float vignetteSmoothess)
        {
            this.vignetteSmoothess = Mathf.Clamp(vignetteSmoothess, 0.0f, 1.0f);
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
            return vignetteSmoothess;
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
        /// Reset the values of the vignette
        /// </summary>
        public void ResetVignette()
        {
            resetVignette.Invoke();
        }
    }
}
