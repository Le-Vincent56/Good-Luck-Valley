using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu
{
    public class LoadController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private MenuController menuController;
        [SerializeField] private Image deleteProgressPanel;
        [SerializeField] private Image deleteProgressAnimation;
        [SerializeField] private Animator deleteProgressAnimator;
        private Dictionary<string, ConfirmationPopupMenu> popUps = new Dictionary<string, ConfirmationPopupMenu>();
        private SaveSlot selectedSlot;
        #endregion

        #region FIELDS
        private const string DELETE_IDLE = "Delete Idle";
        private const string DELETE_ACTIVE = "Delete Active";
        #endregion

        public void Start()
        {
            // Get a list of the pop up menus
            List<ConfirmationPopupMenu> popUpMenus = GetComponentsInChildren<ConfirmationPopupMenu>().ToList();

            // Add pop up menus to the dictionary
            foreach(ConfirmationPopupMenu popUpMenu in popUpMenus)
            {
                if (popUpMenu.gameObject.name.Contains("New Game"))
                    popUps.Add("New Game", popUpMenu);
                else if (popUpMenu.gameObject.name.Contains("Delete"))
                    popUps.Add("Delete", popUpMenu);
            }

            foreach(KeyValuePair<string, ConfirmationPopupMenu> kvp in popUps)
            {
                kvp.Value.DeactivateMenu();
            }

            selectedSlot = null;
        }

        public bool GetIfSlotSelected()
        {
            // If there's no slot, return false
            if (selectedSlot == null) return false;

            // Return true
            return true;
        }

        public void SetSlot(SaveSlot saveSlot)
        {
            // Set the selected slot
            selectedSlot = saveSlot;

            // Check if there's a save slot
            if (selectedSlot == null)
            {
                // Update buttons and return if not
                menuController.LoadState.CheckButtons(false);
                return;
            }

            // Check button interactability based on if the slot has data
            if (selectedSlot.IsEmpty)
            {
                menuController.LoadState.CheckButtons(false);
            } else
            {
                menuController.LoadState.CheckButtons(true);
            }
        }

        public void StartGame()
        {
            // If no slot is selected, return
            if (selectedSlot == null) return;

            if(selectedSlot.IsEmpty)
            {
                menuController.LoadState.NewData(selectedSlot);
            } else
            {
                menuController.LoadState.LoadData(selectedSlot);
            }
        }

        public void OpenDeleteConfirmation()
        {
            // Activate the delete confirmation menu
            popUps["Delete"].ActivateMenu("Are you sure you want to delete this save?",
                () => // Confirm
                {
                    // Delete the data
                    menuController.LoadState.DeleteData(selectedSlot);

                    // Show the overlay
                    ShowDeleteOverlay();
                },
                () => { } // Cancel
            );
        }

        /// <summary>
        /// Show the delete overlay
        /// </summary>
        private void ShowDeleteOverlay()
        {
            // Show saving in progress UI elements and set saving text update timer
            deleteProgressPanel.enabled = true;
            deleteProgressAnimation.enabled = true;
            deleteProgressAnimator.Play(DELETE_ACTIVE);

            // Set a delay for saving before reloading the menu to allow time for the new save to appear
            StartCoroutine(DeleteDelay());
        }

        /// <summary>
        /// Progress the delete loading screen
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeleteDelay()
        {
            // Fade in UI elements
            yield return StartCoroutine(FadeInProgressUI());

            // Start the timer for saving
            yield return StartCoroutine(DeleteTimer());

            // Fade out UI elements
            yield return StartCoroutine(FadeOutProgressUI());

            // Reload the state
            menuController.LoadState.Reload();

            yield return null;
        }

        /// <summary>
        /// Hold the deleting panel and text for a given time
        /// </summary>
        /// <returns></returns>
        private IEnumerator DeleteTimer()
        {
            yield return new WaitForSecondsRealtime(1.66730f);

            // Play sound

            yield return new WaitForSecondsRealtime(1.3327f);
        }

        /// <summary>
        /// Fade in the Deleting in Progress UI
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInProgressUI()
        {
            // While alpha values are under the desired numbers, increase them by an unscaled delta time (because we are paused)
            while (deleteProgressPanel.color.a < 0.67 && deleteProgressAnimation.color.a < 1)
            {
                deleteProgressPanel.color = new Color(deleteProgressPanel.color.r, deleteProgressPanel.color.g, deleteProgressPanel.color.b, deleteProgressPanel.color.a + (Time.unscaledDeltaTime * 2));
                deleteProgressAnimation.color = new Color(deleteProgressAnimation.color.r, deleteProgressAnimation.color.g, deleteProgressAnimation.color.b, deleteProgressAnimation.color.a + (Time.unscaledDeltaTime * 3f));
                yield return null;
            }
        }

        /// <summary>
        /// Fade out the Deleting in Progress UI
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutProgressUI()
        {
            // While alpha values are over the desired numbers, decrease them by an unscaled delta time (because we are paused)
            while (deleteProgressPanel.color.a > 0 && deleteProgressAnimation.color.a > 0)
            {
                deleteProgressPanel.color = new Color(deleteProgressPanel.color.r, deleteProgressPanel.color.g, deleteProgressPanel.color.b, deleteProgressPanel.color.a - (Time.unscaledDeltaTime * 2));
                deleteProgressAnimation.color = new Color(deleteProgressAnimation.color.r, deleteProgressAnimation.color.g, deleteProgressAnimation.color.b, deleteProgressAnimation.color.a - (Time.unscaledDeltaTime * 3f));

                if (deleteProgressPanel.color.a <= 0 && deleteProgressAnimation.color.a <= 0)
                {
                    // Disable the UI elements once they are gone
                    deleteProgressAnimator.Play(DELETE_IDLE);
                    deleteProgressAnimation.enabled = false;
                    deleteProgressPanel.enabled = false;
                }
            }

            yield return null;
        }
    }
}