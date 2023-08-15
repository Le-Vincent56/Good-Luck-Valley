using UnityEngine;
using UnityEngine.InputSystem;

namespace HiveMind.UI
{
    public class UIManager : MonoBehaviour
    {
        #region REFERENCES
        private Canvas pauseUI;
        #endregion

        #region FIELDS
        private bool paused = false;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            pauseUI = GameObject.Find("PauseUI").GetComponent<Canvas>();
            pauseUI.enabled = false;
        }

        /// <summary>
        /// Pause the Game
        /// </summary>
        /// <param name="context">The context of the Controller</param>
        public void Pause(InputAction.CallbackContext context)
        {
            if (!paused)
            {
                paused = true;
                pauseUI.enabled = true;
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                pauseUI.enabled = false;
                Time.timeScale = 1;
            }
        }
    }
}
