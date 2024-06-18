using GoodLuckValley.Audio.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GoodLuckValley.UI
{
    public class ConfirmationPopupMenu : MonoBehaviour
    {
        [SerializeField] private SoundData buttonSFX;
        [SerializeField] private Text displayText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        /// <summary>
        /// Show the popup menu
        /// </summary>
        /// <param name="displaytext">The display text</param>
        /// <param name="confirmAction">The code that will run on confirmation</param>
        /// <param name="cancelAction">The code that will run on cancellation</param>
        public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
        {
            gameObject.SetActive(true);

            // Set the display text
            this.displayText.text = displayText;

            // Remove any existening listeners just to make sure there aren't any previous ones hanging around
            // This only removes listeners added through code
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            // Assign the onClick listeners
            confirmButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                confirmAction();
                PlaySound();
            });

            cancelButton.onClick.AddListener(() =>
            {
                DeactivateMenu();
                cancelAction();
                PlaySound();
            });
        }

        public void DeactivateMenu()
        {
            gameObject.SetActive(false);
        }

        public void PlaySound()
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithSoundData(buttonSFX)
                .WithRandomPitch()
                .Play();
        }
    }
}