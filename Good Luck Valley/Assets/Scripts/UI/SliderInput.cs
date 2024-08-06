using GoodLuckValley.Player.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Elements
{
    public class SliderInput : Slider
    {
        [Header("References")]
        [SerializeField] private MenuInputReader menuInputReader;

        [Header("Fields")]
        [SerializeField] private float normalStep = 1f;
        [SerializeField] private bool applyAltMod = false;
        [SerializeField] private float altStep = 5f;
        [SerializeField] private bool applyShiftMod = false;
        [SerializeField] private float shiftStep = 10f;        

        protected override void OnEnable()
        {
            base.OnEnable();

            menuInputReader.AltModifier += EnableAltModifier;
            menuInputReader.ShiftModifier += EnableShiftModifier;
            menuInputReader.Navigate += NavigateSlider;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            menuInputReader.AltModifier -= EnableAltModifier;
            menuInputReader.ShiftModifier -= EnableShiftModifier;
            menuInputReader.Navigate -= NavigateSlider;
        }

        /// <summary>
        /// Adjust the slider value
        /// </summary>
        /// <param name="moveDirection"></param>
        private void AdjustSliderValue(MoveDirection moveDirection)
        {
            float step = normalStep;

            if (applyAltMod)
                step = altStep;

            if (applyShiftMod)
                step = shiftStep;

            // Calculate in which direction to apply the value towards
            float finalStep = (moveDirection == MoveDirection.Left)
                ? -step
                : step;

            // Apply the final value
            value += finalStep;
        }

        /// <summary>
        /// Enable the Alt modifier for the slider
        /// </summary>
        /// <param name="started"></param>
        private void EnableAltModifier(bool started)
        {
            // Check if the modifier has been pressed
            if (started) 
            {
                // Override shift modifier if pressed afterwards
                if (applyShiftMod)
                    applyShiftMod = false;

                applyAltMod = true;
            } else
            {
                // Don't apply the alt mod if not pressed
                applyAltMod = false;
            }
        }

        /// <summary>
        /// Enable the Shift modifier for the slider
        /// </summary>
        /// <param name="started"></param>
        private void EnableShiftModifier(bool started)
        {
            // Check if the modifier has been pressed
            if (started)
            {
                // Override alt modifier if pressed afterwards
                if (applyAltMod)
                    applyAltMod = false;

                applyShiftMod = true;
            } else
            {
                // Don't apply the shift mod if not pressed
                applyShiftMod = false;
            }
        }

        /// <summary>
        /// Navigate the slider using input
        /// </summary>
        /// <param name="navigation"></param>
        private void NavigateSlider(Vector2 navigation)
        {
            // Exit case, there is no x input
            if (navigation.x == 0) return;

            // Exit case - the slider is not selected
            if (EventSystem.current.currentSelectedGameObject != gameObject) return;

            // Get the move direction
            MoveDirection moveDirection = (navigation.x < 0) ? MoveDirection.Left : MoveDirection.Right;

            // Adjust the slider value
            AdjustSliderValue(moveDirection);
        }
    }
}