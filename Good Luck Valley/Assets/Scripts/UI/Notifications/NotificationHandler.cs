using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Player.Input;
using GoodLuckValley.Events;

namespace GoodLuckValley.UI.Notifications
{
    public struct NotificationData
    {
        public NotificationHandler.Type Type;
        public int JournalPageIndex;

        public NotificationData(NotificationHandler.Type type, int journalIndex)
        {
            Type = type;
            JournalPageIndex = journalIndex;
        }
    }

    public class NotificationHandler : MonoBehaviour
    {
        public enum Type
        {
            Journal,
            Note,
            Entry,
        }

        [Header("Events")]
        [SerializeField] private GameEvent onOpenJournalNotification;

        [Header("References")]
        [SerializeField] private InputReader defaultInputReader;
        [SerializeField] private NotificationPanel notificationPanel;
        [SerializeField] private JournalNotification currentNotification;

        [Header("Fields")]
        Queue<JournalNotification> notificationQueue = new Queue<JournalNotification>();
        [SerializeField] private float currentNotificationTime;
        [SerializeField] private float dequeuingProgress;
        [SerializeField] private float dequeueTime;
        [SerializeField] private bool canOpen;
        [SerializeField] private float holdInputProgress;
        [SerializeField] private float holdInputTime;

        private void OnEnable()
        {
            defaultInputReader.OpenJournal += HoldToOpen;
        }

        private void OnDisable()
        {
            defaultInputReader.OpenJournal -= HoldToOpen;
        }

        private void Update()
        {
            // Check if in dequeuing time
            if (dequeuingProgress < dequeueTime)
            {
                // If so, continue dequeuing
                dequeuingProgress += Time.deltaTime;
                return;
            }

            // Exit case - there's nothing in the notification queue
            if (notificationQueue.Count <= 0 || Journal.Journal.Instance.Open) return;

            // Check if there's a current notification
            if(currentNotification == null)
            {
                // If not, then get the first notification in the queue
                currentNotification = notificationQueue.Peek();
                Journal.Journal.Instance.SetNotificationInput(true);
            }

            // Check if the notification is finished
            if (currentNotification.Finished)
            {
                // Clear the notification
                ClearNotification();
                return;
            }

            // Update the current notification
            currentNotification.Update(Time.deltaTime);

            // Get the time for debuggin
            currentNotificationTime = currentNotification.GetTime();
        }

        /// <summary>
        /// Clear a notification
        /// </summary>
        private void ClearNotification(bool forceClear = false)
        {
            // Check if force clearing
            if(forceClear)
                currentNotification.ForceClear();

            // Dequeue the notification queue
            notificationQueue.Dequeue();

            // Set the current notification to null
            currentNotification = null;
            
            // Start the dequeuing process
            dequeuingProgress = 0f;

            // Disable notification input
            Journal.Journal.Instance.SetNotificationInput(false);
        }

        /// <summary>
        /// Create a notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Notify(Component sender, object data)
        {
            // Verify that the correct data has been sent
            if (data is not NotificationData) return;

            // Cast the data
            NotificationData notificationData = (NotificationData)data;

            // Check the type of notification and send the corresponding type
            switch (notificationData.Type)
            {
                // Notification for Journal pickup
                case Type.Journal:
                    NotifyJournalPickup(notificationData.JournalPageIndex);
                    break;

                // Notification for Note pickup
                case Type.Note:
                    NotifyNotePickup(notificationData.JournalPageIndex);
                    break;

                // Notification for Journal Entry unlock
                case Type.Entry:
                    NotifyEntryUnlock(notificationData.JournalPageIndex);
                    break;
            }
        }

        /// <summary>
        /// Notify the Player of the Journal pickup
        /// </summary>
        private void NotifyJournalPickup(int journalPageIndex)
        {
            // Create the notification
            JournalPickupNotification notification = new JournalPickupNotification(notificationPanel, journalPageIndex);

            // Enqueue the notification
            notificationQueue.Enqueue(notification);
        }

        /// <summary>
        /// Notify the Player of a Note pickup
        /// </summary>
        private void NotifyNotePickup(int journalPageIndex)
        {
            // Create the notification
            NoteNotification notification = new NoteNotification(Journal.Journal.Instance.GetNumOfNotesCollected(), notificationPanel, journalPageIndex);

            // Enqueue the notification
            notificationQueue.Enqueue(notification);
        }

        /// <summary>
        /// Notify the Player of a Journal Entry unlock
        /// </summary>
        private void NotifyEntryUnlock(int journalPageIndex)
        {
            // Create the notification
            EntryNotification notification = new EntryNotification(
                Journal.Journal.Instance.GetEntryAt(
                    Journal.Journal.Instance.GetNumOfUnlockedEntries()
                )
                .Title,
                notificationPanel,
                journalPageIndex
            );

            // Enqueue the notification
            notificationQueue.Enqueue(notification);
        }
        
        /// <summary>
        /// Process holding input for opening the Journal Notifications
        /// </summary>
        /// <param name="started"></param>
        /// <param name="performed"></param>
        private void HoldToOpen(bool started, bool performed)
        {
            if (!Journal.Journal.Instance.NotificationInput) return;

            // Exit case - the button hasn't been lifted
            if (started) return;

            // Open the journal notification
            onOpenJournalNotification.Raise(this, currentNotification.JournalPageIndex);

            // Force clear the notification
            ClearNotification(true);
        }
    }
}