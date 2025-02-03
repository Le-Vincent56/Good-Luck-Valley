using DG.Tweening;
using GoodLuckValley.UI.Journal.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.View
{
    public class EntryButton : MonoBehaviour
    {
        [Header("References")]
        private Button button;
        [SerializeField] private CanvasGroup activeGroup;
        [SerializeField] private CanvasGroup inactiveGroup;
        [SerializeField] private Text titleText;

        [Header("Fields")]
        [SerializeField] private TabType tab;
        [SerializeField] private int index;
        [SerializeField] private string content;

        [Header("Tweening Variables")]
        [SerializeField] private Ease easeType;
        [SerializeField] private float fadeDuration;
        private Tween activeGroupTween;
        private Tween inactiveGroupTween;

        public event Action<EntryButton> OnEntryClicked = delegate { };
        public string Content { get => content; }

        public int Index { get => index; }
        public TabType Tab { get => tab; }
        public string TitleText { get { return titleText.text; } }
        public bool Interactable { get => button.interactable; }

        private void OnDestroy()
        {
            // Kill all existing Tweens
            activeGroupTween?.Kill();
            inactiveGroupTween?.Kill();
        }

        /// <summary>
        /// Initialize the Entry Button
        /// </summary>
        public void Initialize(JournalView view)
        {
            // Get components
            button = GetComponent<Button>();
            titleText = activeGroup.GetComponentInChildren<Text>();

            // Add event listeners
            button.onClick.AddListener(() => OnEntryClicked(this));

            // Initialize the Entry Button
            EntryButtonEffect effect = GetComponent<EntryButtonEffect>();
            effect.Initialize(this, view);
        }

        /// <summary>
        /// Register a listener to the OnEntryClicked event
        /// </summary>
        public void RegisterListener(Action<EntryButton> listener) => OnEntryClicked += listener;

        /// <summary>
        /// Deregister a listener from the OnEntryClicked event
        /// </summary>
        public void DeregisterListener(Action<EntryButton> listener) => OnEntryClicked -= listener;

        /// <summary>
        /// Set the index of the Entry Button
        /// </summary>
        public void SetIndex(int index) => this.index = index;

        /// <summary>
        /// Set the Tab of the Entry Button
        /// </summary>
        public void SetTab(TabType tab) => this.tab = tab;

        /// <summary>
        /// Set the title of the Entry Button
        /// </summary>
        public void SetTitle(string title)
        {
            titleText.text = title;
        }

        /// <summary>
        /// Set the content of the Entry Button
        /// </summary>
        public void SetContent(string content) => this.content = content;

        /// <summary>
        /// Set the interactability state of the Entry Button
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            // Set whether or not the Button is interactable
            button.interactable = interactable;

            // Check the interactability state
            if(interactable)
            {
                // If interactable, fade in the active group
                Fade(activeGroupTween, activeGroup, 1f, fadeDuration, Ease.InOutSine);
                Fade(inactiveGroupTween, inactiveGroup, 0f, fadeDuration, Ease.InOutSine);
            } else
            {
                // Oterhwise, fade in the inactive group
                Fade(activeGroupTween, activeGroup, 0f, fadeDuration, Ease.InOutSine);
                Fade(inactiveGroupTween, inactiveGroup, 1f, fadeDuration, Ease.InOutSine);
            }
        }

        public void Select() { }
        public void Deselect() { }

        private void Fade(Tween fadeTween, CanvasGroup canvasGroup, 
            float endvalue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the existing Fade Tween
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endvalue, duration);

            // Set the Ease Type
            fadeTween.SetEase(easeType);

            // Exit case - if there is no completion actoin
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}
