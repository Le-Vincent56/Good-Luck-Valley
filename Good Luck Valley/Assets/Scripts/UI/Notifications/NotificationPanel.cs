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
        [SerializeField] private Image keyPressImage;
        private bool hidden;

        // Down the line, think about having a ScriptableObject that holds a list for each sprite,
        // then use a Dictionary here to select indexes (select in Awake)
        [SerializeField] private Sprite unpressedImage;
        [SerializeField] private Sprite pressedImage;

        public bool Hidden { get => hidden; }

        protected override void Awake()
        {
            base.Awake();
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
        }

        /// <summary>
        /// Hide the Notification Panel UI
        /// </summary>
        public override void HideUI()
        {
            base.HideUI();
            hidden = true;
        }
    }
}