using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class TextBlinker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Text textToBlink;
        [SerializeField] private Color startingColor;

        [Header("Fields")]
        [SerializeField] private float blinkTime;
        [SerializeField] private float blinkMax;
        [SerializeField] private bool increaseTime;

        private void Start()
        {
            // Set the starting color to invisible
            startingColor = textToBlink.color;
            startingColor.a = 0f;
            textToBlink.color = startingColor;

            // Set initial time
            blinkTime = 1f;
            increaseTime = false;
        }

        private void Update()
        {
            // Exit case - if the text to blink or its game object is not enabled
            if (!textToBlink.enabled || !textToBlink.gameObject.activeSelf) return;

            // Check if already increasing time and over the max
            if (increaseTime && blinkTime >= blinkMax)
            {
                // Set increase time to false
                increaseTime = false;
            }
            // Otherwise, check if increaseTime is false and blinkTime is less than 0
            else if (!increaseTime && blinkTime <= 0)
            {
                // Set increase time to true
                increaseTime = true;
            }

            // Check if the time should be increasing
            if (increaseTime)
            {
                // Add deltaTime
                blinkTime += Time.deltaTime;
            }
            else if (!increaseTime)
            {
                // Otherwise subtract deltaTime
                blinkTime -= Time.deltaTime;
            }

            // Set the starting color's opacity
            startingColor.a = blinkTime / blinkMax;

            // Set the color
            textToBlink.color = startingColor;
        }
    }
}