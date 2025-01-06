using GoodLuckValley.Architecture.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class DevToolsDebugger : MonoBehaviour
    {
        [SerializeField] private Text activeText;
        [SerializeField] private Text toolsText;

        private EventBinding<ToggleDevelopmentTools> onToggleDevelopmentTools;
        private EventBinding<ChangeDevelopmentTools> onChangeDevelopmentTools;

        private void Start()
        {
            // Set the texts to empty strings
            activeText.text = string.Empty;
            toolsText.text = string.Empty;
        }

        private void OnEnable()
        {
            // Create and register Event Bindings
            onToggleDevelopmentTools = new EventBinding<ToggleDevelopmentTools>(UpdateActiveText);
            EventBus<ToggleDevelopmentTools>.Register(onToggleDevelopmentTools);

            onChangeDevelopmentTools = new EventBinding<ChangeDevelopmentTools>(UpdateToolsText);
            EventBus<ChangeDevelopmentTools>.Register(onChangeDevelopmentTools);
        }

        private void OnDisable()
        {
            // Deregister Event Bindings
            EventBus<ToggleDevelopmentTools>.Deregister(onToggleDevelopmentTools);
            EventBus<ChangeDevelopmentTools>.Deregister(onChangeDevelopmentTools);
        }

        /// <summary>
        /// Update the Development Tools Active Text
        /// </summary>
        private void UpdateActiveText(ToggleDevelopmentTools eventData)
        {
            // Exit case - not debugging Development Tools
            if (!eventData.Debug) return;

            // Set the appropriate text
            activeText.text = eventData.Active ? "Development Tools" : string.Empty;

            // List the tools
            ListTools(eventData.NoClip);

            // If not active, clear the active Tools text
            if (!eventData.Active) toolsText.text = string.Empty;
        }

        /// <summary>
        /// Udpate the Developemnt Tools text
        /// </summary>
        private void UpdateToolsText(ChangeDevelopmentTools eventData)
        {
            // Exit case - not debugging Development Tools
            if (!eventData.Debug) return;

            // List the tools
            ListTools(eventData.NoClip);
        }

        /// <summary>
        /// List the tools and their state
        /// </summary>
        private void ListTools(bool noClip)
        {
            // Create a container text (in case more tools are added to be debugged later)
            string textToSet = "";

            // Add NoClip debug text
            textToSet += (noClip) ? "NoClip: Active" : "NoClip: Inactive";

            // Set the text
            toolsText.text = textToSet;
        }
    }
}
