using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Notifications
{
    public class NotificationPanel : FadePanel
    {
        [Header("References")]
        [SerializeField] private Text headerText;
        [SerializeField] private Text descriptorText;
        [SerializeField] private FadePanel radial;
        [SerializeField] private Image keyPressImage;
        [SerializeField] private RadialProgress radialProgress;
        private bool hidden;

        // Down the line, think about having a ScriptableObject that holds a list for each sprite,
        // then use a Dictionary here to select indexes (select in Awake)
        [SerializeField] private Sprite unpressedImage;
        [SerializeField] private Sprite pressedImage;

        public bool Hidden { get => hidden; }

        protected override void Awake()
        {
            base.Awake();

            // Set the fade duration to sync with the Notification Panel
            radial.SetFadeDuration(fadeDuration);
        }

        public void SetText(string headerText, string descriptorText)
        {
            this.headerText.text = headerText;
            this.descriptorText.text = descriptorText;
        }

        /// <summary>
        /// Show the Notification Panel UI
        /// </summary>
        public override void ShowUI()
        {
            hidden = false;
            base.ShowUI();
            radial.ShowUI();
        }

        /// <summary>
        /// Hide the Notification Panel UI
        /// </summary>
        public override void HideUI()
        {
            base.HideUI();
            radial.HideUI();
            hidden = true;
        }

        /// <summary>
        /// Update the progress of the radial progress fill
        /// </summary>
        /// <param name="progressTotal">The total amount of progress done</param>
        public void UpdateProgress(float progressTotal) => radialProgress.UpdateProgress(progressTotal);

        /// <summary>
        /// Set the "Key Pressed" sprite
        /// </summary>
        public void SetKeyPressed()
        {
            if(keyPressImage.sprite != pressedImage)
                keyPressImage.sprite = pressedImage;

        }

        /// <summary>
        /// Set the "Key Unpressed" sprite
        /// </summary>
        public void SetKeyUnpressed()
        {
            if (keyPressImage.sprite != unpressedImage)
                keyPressImage.sprite = unpressedImage;
        }
    }
}