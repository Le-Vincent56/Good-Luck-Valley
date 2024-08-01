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
        }

        public void BackToMain()
        {
            SceneLoader.Instance.LoadMainMenu();
        }
    }
}