using GoodLuckValley.Audio.Ambience;
using GoodLuckValley.Audio.Music;
using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using GoodLuckValley.SceneManagement;
using GoodLuckValley.UI.Menus;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoodLuckValley.UI.EndScreen
{
    public class EndScreenController : MonoBehaviour
    {
        [SerializeField] private MenuInputReader inputReader;
        private MenuCursor cursors;

        private void Awake()
        {
            cursors = GetComponent<MenuCursor>();
        }

        private void Start()
        {
            cursors.ShowCursors();
            cursors.ActivateCursors();

            inputReader.Enable();

            AmbienceManager.Instance.StopAmbience();
            MusicManager.Instance.SetMenuStates();
        }

        public void ResumePlaying()
        {
            // Set game states
            MusicManager.Instance.SetGameStates();

            // Load the scene
            SceneLoader.Instance.EnterGame(SaveLoadSystem.Instance.selectedData.CurrentLevelName);
        }

        public void BackToMain()
        {
            SceneLoader.Instance.LoadMainMenu();
        }
    }
}