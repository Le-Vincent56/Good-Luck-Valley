using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Journal
{
    public class NoteButton : MonoBehaviour
    {
        public string Title;
        public int Index;
        public event Action<int> OnButtonPressed = delegate { };

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnButtonPressed(Index));
        }

        public void Initialize(string title, int index)
        {
            Title = title;
            Index = index;
        }

        public void RegisterListener(Action<int> listener)
        {
            OnButtonPressed += listener;
        }
    }
}