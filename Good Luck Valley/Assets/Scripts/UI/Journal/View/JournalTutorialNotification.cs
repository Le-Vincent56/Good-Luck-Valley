using GoodLuckValley.Events.Journal;
using GoodLuckValley.Events;
using UnityEngine;
using GoodLuckValley.Timers;
using DG.Tweening;
using GoodLuckValley.Input;

namespace GoodLuckValley.UI.Journal.View
{
    public class JournalTutorialNotification : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Notification Fields")]
        [SerializeField] private float notificationTime;
        private CountdownTimer notificationTimer;

        [Header("Tweening Variables")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<UnlockJournal> onAddJournalEntry;

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
            inputReader.Journal += HideNotification;

            onAddJournalEntry = new EventBinding<UnlockJournal>(Notify);
            EventBus<UnlockJournal>.Register(onAddJournalEntry);
        }

        private void OnDisable()
        {
            inputReader.Journal -= HideNotification;

            EventBus<UnlockJournal>.Deregister(onAddJournalEntry);
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
        private void Notify(UnlockJournal unlockJournalEntry) => Show();

        /// <summary>
        /// Hide the notification is the journal is activated
        /// </summary>
        private void HideNotification(bool started)
        {
            // Exit case - if the button is being held down
            if (started) return;

            // Hide the Notification
            notificationTimer?.Stop();
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
