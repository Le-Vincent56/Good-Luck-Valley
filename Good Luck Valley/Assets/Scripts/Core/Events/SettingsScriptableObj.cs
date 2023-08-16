using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HiveMind.Events
{
    [CreateAssetMenu(fileName = "SettingsScriptableObject", menuName = "ScriptableObjects/Settings Event")]
    public class SettingsScriptableObj : ScriptableObject
    {
        #region FIELDS
        [SerializeField] private bool noClipActive;
        [SerializeField] private bool instantThrowActive;
        [SerializeField] private int throwMultiplier;
        [SerializeField] private bool infiniteShroomsActive;
        [SerializeField] private int shroomLimit;
        [SerializeField] private bool shroomTimersActive;
        [SerializeField] private bool throwLineActive;

        #region EVENTS
        public UnityEvent updateSettings;
        #endregion
        #endregion

        private void OnEnable()
        {
            #region CREATE EVENTS
            if(updateSettings == null)
            {
                updateSettings = new UnityEvent();
            }
            #endregion
        }

        /// <summary>
        /// Set whether no clip is active or not
        /// </summary>
        /// <param name="noClipActive">Whether no clip is active or not</param>
        public void SetNoClipActive(bool noClipActive)
        {
            this.noClipActive = noClipActive;
        }

        /// <summary>
        /// Set whether instant throw is active or not
        /// </summary>
        /// <param name="instantThrowActive">Whether instant throw is active or not</param>
        public void SetInstantThrowActive(bool instantThrowActive)
        {
            this.instantThrowActive = instantThrowActive;
        }

        /// <summary>
        /// Set the shroom multiplier
        /// </summary>
        /// <param name="throwMultiplier">The shroom multiplier</param>
        public void SetThrowMultiplier(int throwMultiplier)
        {
            this.throwMultiplier = throwMultiplier;
        }

        /// <summary>
        /// Set whether infinite shrooms are active or not
        /// </summary>
        /// <param name="infiniteShroomsActive">Whether infinite shrooms are active or not</param>
        public void SetInfiniteShroomsActive(bool infiniteShroomsActive)
        {
            this.infiniteShroomsActive= infiniteShroomsActive;
        }

        /// <summary>
        /// Set whether infinite shrooms are active or not
        /// </summary>
        /// <param name="shroomLimit">Whether infinite shrooms are active or not</param>
        public void SetShroomLimit(int shroomLimit)
        {
            this.shroomLimit = shroomLimit;
        }

        /// <summary>
        /// Set whether the shroom timers are active or not
        /// </summary>
        /// <param name="shroomTimersActive">Whether the shroom timers are active or not</param>
        public void SetShroomTimersActive(bool shroomTimersActive)
        {
            this.shroomTimersActive = shroomTimersActive;
        }

        /// <summary>
        /// Set whether the shroom throw line is active or not
        /// </summary>
        /// <param name="throwLineActive">Whether the shroom throw line is active or not</param>
        public void SetThrowLineActive(bool throwLineActive)
        {
            this.throwLineActive = throwLineActive;
        }

        /// <summary>
        /// Get whether no clip is active or not
        /// </summary>
        /// <returns>Whether no clip is active or not</returns>
        public bool GetNoClipActive()
        {
            return noClipActive;
        }

        /// <summary>
        /// Get whether instant throw is active or not
        /// </summary>
        /// <returns>Whether instant throw is active or not</returns>
        public bool GetInstantThrowActive()
        {
            return instantThrowActive;
        }

        /// <summary>
        /// Get the shroom throw multiplier
        /// </summary>
        /// <returns>The shroom throw multiplier</returns>
        public int GetThrowMultiplier()
        {
            return throwMultiplier;
        }

        /// <summary>
        /// Get whether infinite shrooms are active or not
        /// </summary>
        /// <returns>Whether infinite shrooms are active or not</returns>
        public bool GetInfiniteShroomsActive()
        {
            return infiniteShroomsActive;
        }

        /// <summary>
        /// Get whether infinite shrooms are active or not
        /// </summary>
        /// <returns>Whether infinite shrooms are active or not</returns>
        public int GetShroomLimit()
        {
            return shroomLimit;
        }

        /// <summary>
        /// Get whether shroom timers are active or not
        /// </summary>
        /// <returns>Whether shroom timers are active or not</returns>
        public bool GetShroomTimersActive()
        {
            return shroomTimersActive;
        }

        /// <summary>
        /// Get whether the throw line is active or not
        /// </summary>
        /// <returns>Whether the throw line is active or not</returns>
        public bool GetThrowLineActive()
        {
            return throwLineActive;
        }

        /// <summary>
        /// Trigger events related to changing shroom settings
        /// </summary>
        public void UpdateShroomSettings()
        {
            updateSettings.Invoke();
        }

        /// <summary>
        /// Reset object variables
        /// </summary>
        public void ResetObj()
        {
            throwMultiplier = 8;
            shroomLimit = 3;
            instantThrowActive = false;
            infiniteShroomsActive = false;
            shroomTimersActive = true;
            throwLineActive = true;
        }
    }
}
