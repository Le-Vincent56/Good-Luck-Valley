using DG.Tweening;
using GoodLuckValley.Extensions.GameObjects;
using GoodLuckValley.UI.Journal.Model;
using GoodLuckValley.Utilities.Preconditions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.View
{
    public class JournalView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CanvasGroup menuGroup;
        [SerializeField] private CanvasGroup entriesGroup;
        [SerializeField] private CanvasGroup contentGroup;
        [SerializeField] private Text contentTitle;
        [SerializeField] private Text contentText;
        private EntryButtonEffect lastSelectedEntry;
        private TabButtonEffect lastSelectedTab;
        [SerializeField] private TabButton[] tabs;
        [SerializeField] private TabButtonEffect[] tabEffects;
        [SerializeField] private EntryButton[] entries;


        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        [SerializeField] private float entryFadeDuration;
        [SerializeField] private float contentFadeDuration;
        private Tween canvasFadeTween;
        private Tween entriesFadeTween;
        private Tween contentFadeTween;

        public EntryButton[] Entries { get => entries; }
        public TabButton[] Tabs { get => tabs; }
        public TabButtonEffect[] Effects { get => tabEffects; }
        public EntryButtonEffect LastSelectedEntry { get => lastSelectedEntry; set => lastSelectedEntry = value; }
        public TabButtonEffect LastSelectedTab { get => lastSelectedTab; set => lastSelectedTab = value; }

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
            tabs = menuGroup.GetComponentsInChildren<TabButton>();
            tabEffects = menuGroup.GetComponentsInChildren<TabButtonEffect>();
            entries = menuGroup.GetComponentsInChildren<EntryButton>();

            // Set the texts
            Text[] contentTexts = contentGroup.GetComponentsInChildren<Text>();
            contentTitle = contentTexts[0];
            contentText = contentTexts[1];

            // Iterate through each Tab button
            for(int i = 0; i < tabs.Length; i++)
            {
                // Initialize each Tab Button
                tabs[i].Initialize(this);
            }

            // Iterate through each Entry Button
            for (int i = 0; i < entries.Length; i++)
            {
                // Initialize each Entry Button
                entries[i].Initialize(this);
            }

            // Fade out the content
            Fade(contentFadeTween, contentGroup, 0f, 0f);
        }

        /// <summary>
        /// Update the Journal Entries
        /// </summary>
        public void UpdateEntries(IList<JournalEntry> journalEntries)
        {
            // Iterate through each Entry Button
            for(int i = 0; i < entries.Length;i++)
            {
                // Skip if the index is greater than the length of the List
                if(i >= journalEntries.Count)
                {
                    // Deactivate the Entry Button
                    entries[i].gameObject.SetActive(false);
                    continue;
                }

                // Activate the Entry Button
                entries[i].gameObject.SetActive(true);

                // Set the interactability state of the Entry Button
                entries[i].SetInteractable(journalEntries[i].Unlocked);

                // Set the data
                entries[i].SetIndex(journalEntries[i].Data.Index);
                entries[i].SetTab(journalEntries[i].Data.Tab);
                entries[i].SetTitle(journalEntries[i].Data.Title);
                entries[i].SetContent(journalEntries[i].Data.Content);
            }

            // Iterate through each Tab Button
            for(int i = 0; i < tabs.Length; i++)
            {
                // Default that a Tab has no Entries
                bool hasEntries = false;

                // Iterate through each Entry Button
                for(int j = 0; j < entries.Length; j++)
                {
                    // Check if the Entry Button's Tab matches the Tab Button's Tab
                    if (entries[j].Tab == tabs[i].Tab)
                    {
                        // If so, notify that the Tab has Journal Entries
                        hasEntries = true;
                        break;
                    }
                }

                // Set the Tab Button's interactability to match whether or not
                // it has entries
                tabs[i].SetInteractable(hasEntries);
            }

            // Show the Tab Entries of the Diary
            ShowTabEntries(TabType.Diary);
        }

        /// <summary>
        /// Show all Entries of a certain Tab
        /// </summary>
        public void ShowTabEntries(TabType tabType)
        {
            // fade out
            Fade(entriesFadeTween, entriesGroup, 0f, entryFadeDuration / 2f, () =>
            {
                // Iterate through each Entry Button
                for (int i = 0; i < entries.Length; i++)
                {
                    // Deactivate the Entry Button
                    entries[i].gameObject.SetActive(false);

                    // Skip if the Entry Button's tab and the given tab are not equal
                    if (entries[i].Tab != tabType) continue;

                    // Activate the Entry Button
                    entries[i].gameObject.SetActive(true);
                }

                // Correct the Tabs
                CorrectTabs(tabType);

                // Fade back in
                Fade(entriesFadeTween, entriesGroup, 1f, entryFadeDuration / 2f);
            });
        }

        /// <summary>
        /// Show the content of an Entry
        /// </summary>
        public void ShowEntryContent(string title, string content)
        {
            // Fade out
            Fade(contentFadeTween, contentGroup, 0f, contentFadeDuration / 2f, () =>
            {
                // Set the content
                contentTitle.text = title;
                contentText.text = content;

                // Fade back in
                Fade(contentFadeTween, contentGroup, 1f, contentFadeDuration / 2f);
            });
        }

        /// <summary>
        /// Update the viewed content
        /// </summary>
        public void UpdateContent(string content) => contentText.text = content;

        /// <summary>
        /// Correct the Tabs to only select the last selected Tab
        /// </summary>
        public void CorrectTabs(TabType tab)
        {
            // Iterate through each Tab Effect Button
            for(int i = 0; i < tabEffects.Length; i++)
            {
                // Check if the Tab Button's Tab equals the given Tab
                if (tabEffects[i].GetComponent<TabButton>().Tab == tab)
                {
                    // Select the Tab Effect Button
                    tabEffects[i].Select();

                    continue;
                }

                // Deselect the Tab Button Effect
                tabEffects[i].Deselect();
            }
        }

        /// <summary>
        /// Select the last selected Tab
        /// </summary>
        public void SelectLastTab()
        {
            // Exit case - if there is no last selected Tab
            if (lastSelectedTab == null)
            {
                // Select the first Tab
                EventSystem.current.SetSelectedGameObject(tabs[0].gameObject);

                return;
            }

            // Select the last selected Tab
            EventSystem.current.SetSelectedGameObject(lastSelectedTab.gameObject);
        }

        /// <summary>
        /// Select the last selected Entry
        /// </summary>
        public void SelectLastEntry()
        {
            // Exit case - if there is no last selected Entry or the last selected Entry is inactivate
            if (lastSelectedEntry == null || !lastSelectedEntry.gameObject.activeSelf)
            {
                // Get the first enabled entry
                GameObject firstEnabledEntry = entries.Where(x => x.gameObject.activeSelf).First().gameObject;

                // Set it as the Event System's currently selected game object
                EventSystem.current.SetSelectedGameObject(firstEnabledEntry);

                return;
            }

            // Select the last selected Entry
            EventSystem.current.SetSelectedGameObject(lastSelectedEntry.gameObject);
        }

        /// <summary>
        /// Show the Journal
        /// </summary>
        public void Show()
        {
            Fade(canvasFadeTween, canvasGroup, 1f, fadeDuration, () =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // Select the last selected Entry
                SelectLastEntry();
            });
        }

        /// <summary>
        /// Hide the Journal
        /// </summary>
        public void Hide()
        {
            Fade(canvasFadeTween, canvasGroup, 0f, fadeDuration, () =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            });
        }

        /// <summary>
        /// Handle Fade Tweening for the Journal
        /// </summary>
        private void Fade(Tween fadeTween, CanvasGroup canvasGroup, float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Set the Fade Tween
            fadeTween = canvasGroup.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up the completion action
            fadeTween.onComplete += onComplete;
        }
    }
}
