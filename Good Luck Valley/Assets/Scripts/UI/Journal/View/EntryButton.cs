using GoodLuckValley.UI.Journal.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.View
{
    public class EntryButton : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private TabType tab;
        [SerializeField] private Text titleText;
        [SerializeField] private string content;

        public event Action<EntryButton> OnEntryClicked = delegate { };
        public string Content { get => content; }

        public int Index { get => index; }
        public TabType Tab { get => tab; }
        public string TitleText { get { return titleText.text; } }

        /// <summary>
        /// Initialize the Entry Button
        /// </summary>
        public void Initialize(JournalView view)
        {
            // Get components
            Button button = GetComponent<Button>();
            titleText = GetComponentInChildren<Text>();

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

        public void Select() { }
        public void Deselect() { }
    }
}
