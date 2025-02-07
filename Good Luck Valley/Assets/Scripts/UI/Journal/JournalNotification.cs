using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.Timers;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.Notifications
{
    public class JournalNotification : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text notificationText;

        [Header("Notification Fields")]
        [SerializeField] private float notificationTime;
        private CountdownTimer notificationTimer;

        [Header("Tweening Variables")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<UnlockJournalEntry> onAddJournalEntry;

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();

            // Create the Notification Timer
            notificationTimer = new CountdownTimer(notificationTime);
            notificationTimer.OnTimerStop += Hide;
        }

        private void OnEnable()
        {
            onAddJournalEntry = new EventBinding<UnlockJournalEntry>(Notify);
            EventBus<UnlockJournalEntry>.Register(onAddJournalEntry);
        }

        private void OnDisable()
        {
            EventBus<UnlockJournalEntry>.Deregister(onAddJournalEntry);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Dispose of the Notification Timer
            notificationTimer?.Dispose();
        }

        /// <summary>
        /// Notify the player of a new Journal Entry
        /// </summary>
        private void Notify(UnlockJournalEntry unlockJournalEntry)
        {
            // Set the Notification text
            notificationText.text = unlockJournalEntry.Data.Title;

            // Show the Notification
            Show();
        }

        /// <summary>
        /// Show the Notification
        /// </summary>
        private void Show() => Fade(1f, fadeDuration, easeType, () => notificationTimer.Start());

        /// <summary>
        /// Hide the Notification
        /// </summary>
        private void Hide() => Fade(0f, fadeDuration, easeType);

        /// <summary>
        /// Handle Fade Tweening for the Notification
        /// </summary>
        private void Fade(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, duration);

            // Set the Ease Type
            fadeTween.SetEase(easeType);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}
