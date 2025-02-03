using GoodLuckValley.UI.Journal.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Journal.View
{
    public class TabButton : MonoBehaviour
    {
        private Button button;
        [SerializeField] private TabType tab;

        public event Action<TabButton> OnTabClicked = delegate { };

        public TabType Tab { get => tab; }
        public bool Interactable { get => button.interactable; }

        /// <summary>
        /// Initialize the Entry Button
        /// </summary>
        public void Initialize(JournalView view)
        {
            // Get components
            button = GetComponent<Button>();

            // Add event listeners
            button.onClick.AddListener(() => OnTabClicked(this));

            // Initialize the Tab Button Effect
            TabButtonEffect effect = GetComponent<TabButtonEffect>();
            effect.Initialize(this, view);
        }

        /// <summary>
        /// Register a listener to the OnEntryClicked event
        /// </summary>
        public void RegisterListener(Action<TabButton> listener) => OnTabClicked += listener;

        /// <summary>
        /// Deregister a listener from the OnEntryClicked event
        /// </summary>
        public void DeregisterListener(Action<TabButton> listener) => OnTabClicked -= listener;

        /// <summary>
        /// Set whether or not the Tab Button is interactable
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            // Set the interactable state of the button
            button.interactable = interactable;

            // TODO: Set the interactable state of the button effect
        }
    }
}
