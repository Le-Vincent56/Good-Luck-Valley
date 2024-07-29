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
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            menuInputReader.AltModifier -= EnableAltModifier;
            menuInputReader.ShiftModifier -= EnableShiftModifier;
        }

        public override void OnMove(AxisEventData eventData)
        {
            // Check if event data is moving left or right
            if(eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right)
            {
                float step = normalStep;

                if (applyAltMod)
                    step = altStep;

                if(applyShiftMod)
                    step = shiftStep;

                // Calculate in which direction to apply the value towards
                float finalStep = (eventData.moveDir == MoveDirection.Left) 
                    ? -step 
                    : step;

                // Apply the final value
                value += finalStep;
            } else
            {
                // Apply normal base movement if not moving left or right
                base.OnMove(eventData);
            }
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
    }
}