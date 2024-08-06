using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.UI.Notifications
{
    public abstract class JournalNotification
    {
        [Header("References")]
        [SerializeField] protected NotificationPanel notificationPanel;

        [Header("Fields")]
        [SerializeField] protected float timer;
        [SerializeField] protected float lifeTime;
        [SerializeField] protected bool tryToHide;
        [SerializeField] protected bool finished;
        [SerializeField] protected bool displayed;
        [SerializeField] protected string headerText;
        [SerializeField] protected string descriptorText;
        [SerializeField] protected int journalPageIndex;

        public bool Finished { get => finished; }
        public bool Displayed { get => displayed; }
        public int JournalPageIndex {  get => journalPageIndex; }

        public JournalNotification(NotificationPanel panel, int journalPageIndex)
        {
            timer = 0f;
            lifeTime = 3.5f;
            finished = false;
            tryToHide = false;
            headerText = "Default Header";
            descriptorText = "Default Description";
            notificationPanel = panel;
            this.journalPageIndex = journalPageIndex;
        }

        /// <summary>
        /// Display the panel for this notification
        /// </summary>
        public void Display()
        {
            // Set the text
            notificationPanel.SetText(headerText, descriptorText);

            // Show the UI
            notificationPanel.ShowUI();
            displayed = true;
        }

        /// <summary>
        /// Update this notification
        /// </summary>
        /// <param name="time"></param>
        public void Update(float time)
        {
            // Check if the UI needs to be displayed
            if (!displayed)
            {
                Display();
            }

            // Check if the timer is still going
            if(timer < lifeTime)
            {
                // Increment the timer and return
                timer += time;
                return;
            }

            // Check if the timer has finished
            if (!tryToHide)
            {
                notificationPanel.HideUI();
                tryToHide = true;
                return;
            }

            // Check if the Notification Panel is hidden and the Notification hasn't finished
            if(notificationPanel.Hidden && !finished)
            {
                // Finish the Notification
                finished = true;
                return;
            }

            return;
        }

        /// <summary>
        /// Force clear the Notification
        /// </summary>
        public void ForceClear()
        {
            timer = lifeTime;
            notificationPanel.HideUI();
            finished = true;
        }

        /// <summary>
        /// Update the radial progress of the held key
        /// </summary>
        /// <param name="progressTotal">The total progress done</param>
        public void UpdateRadialProgress(float progressTotal) => notificationPanel.UpdateProgress(progressTotal);

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void ResetTimer() => timer = 0f;

        /// <summary>
        /// Get the current time of the timer
        /// </summary>
        /// <returns>The current timer value</returns>
        public float GetTime() => timer;

        /// <summary>
        /// Get the Journal page index associated with the Notification
        /// </summary>
        /// <returns>The Journal page index associated with the Notification</returns>
        public int GetPageIndex() => journalPageIndex;
    }
}