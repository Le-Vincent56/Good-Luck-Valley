using GoodLuckValley.Player.Input;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.Control
{
    public interface IInputReader
    {
        void Enable();
        void Disable();
    }

    public class PlayerInputHandler : MonoBehaviour
    {
        private enum CurrentInputReader
        {
            Default,
            Journal,
            Pause
        }

        private List<IInputReader> inputReaders = new List<IInputReader>();

        [Header("References")]
        [SerializeField] private InputReader defaultInputReader;
        [SerializeField] private JournalInputReader journalInputReader;
        [SerializeField] private PauseInputReader pauseInputReader;

        [Header("Fields")]
        [SerializeField] private CurrentInputReader currentInputReader;

        private void Awake()
        {
            // Add all input readers
            inputReaders.Add(defaultInputReader);
            inputReaders.Add(journalInputReader);
            inputReaders.Add(pauseInputReader);
        }

        private void Start()
        {
            // Enable the default input
            EnableDefaultInput(this, null);
        }

        public void EnableDefaultInput(Component sender, object data)
        {
            // Enable the default input reader
            EnableInputReader(defaultInputReader);

            // Set the current reader for debugging
            currentInputReader = CurrentInputReader.Default;
        }

        public void EnableJournalInput(Component sender, object data)
        {
            // Enable the journal input reader
            EnableInputReader(journalInputReader);

            // Set the current reader for debugging
            currentInputReader = CurrentInputReader.Journal;
        }

        public void EnablePauseInput(Component sender, object data)
        {
            // Enable the current input reader
            EnableInputReader(pauseInputReader);

            // Set the current reader for debugging
            currentInputReader = CurrentInputReader.Pause;
        }

        /// <summary>
        /// Enable an InputReader
        /// </summary>
        /// <param name="inputReaderToEnable">The InputReader to enable</param>
        public void EnableInputReader(IInputReader inputReaderToEnable)
        {
            // Exit case - if the InputReaders are not verified
            if(!VerifyInputReaders())
            {
                Debug.Log("Null Input Readers! Check that all are assigned in PlayerInputHandler");
                return;
            }

            // Loop through each InputReader
            foreach (IInputReader reader in inputReaders)
            {
                // If not the reader to enable, disable it
                if (reader != inputReaderToEnable)
                    reader.Disable();
                // Otherwise, enable the InputReader
                else
                    reader.Enable();
            }
        }

        /// <summary>
        /// Verify that all of the InputReaders are set
        /// </summary>
        /// <returns>True if none of the InputReaders are null, false if otherwise</returns>
        public bool VerifyInputReaders()
        {
            // Loop through each InputReader
            foreach(IInputReader reader in inputReaders)
            {
                // If any of the InputReaders were null, return false
                if (reader == null)
                    return false;
            }

            // If we have successfully made it out of the loop, none
            // of the InputReaders were null, so return true
            return true;
        }
    }
}