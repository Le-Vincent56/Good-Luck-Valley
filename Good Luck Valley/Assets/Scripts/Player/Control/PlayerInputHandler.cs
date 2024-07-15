using GoodLuckValley.Player.Input;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private InputReader defaultInputReader;
        [SerializeField] private JournalInputReader journalInputReader;

        public void EnableJournalInput(Component sender, object data)
        {
            // Return if either input readers are null
            if (defaultInputReader == null || journalInputReader == null) return;

            defaultInputReader.Disable();
            journalInputReader.Enable();
        }

        public void DisableJournalInput(Component sender, object data)
        {
            // Return if either input readers are null
            if (defaultInputReader == null || journalInputReader == null) return;

            journalInputReader.Disable();
            defaultInputReader.Enable();
        }
    }
}