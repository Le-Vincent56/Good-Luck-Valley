using UnityEngine;

namespace GoodLuckValley.UI.MainMenuOld
{
    public class CreditsController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private MenuController menuController;
        #endregion

        /// <summary>
        /// Go back to the Main Menu
        /// </summary>
        public void BackButton()
        {
            // Go back to the Main Menu
            menuController.SetState(1);
        }
    }
}
