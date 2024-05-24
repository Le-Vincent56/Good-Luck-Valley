using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Journal
{
    public class NoteButton : MonoBehaviour
    {
        #region FIELDS
        public int index;
        public event Action<int> OnButtonPressed = delegate { };
        #endregion

        public void Initialize(int index)
        {
            this.index = index;
        }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnButtonPressed(index));
        }

        public void RegisterListener(Action<int> listener) => OnButtonPressed += listener;
        public void DeregisterListener(Action<int> listener) => OnButtonPressed -= listener;
    }
}