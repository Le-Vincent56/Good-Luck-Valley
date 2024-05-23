using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Journal
{
    public class JournalTab : MonoBehaviour
    {
        #region FIELDS
        public int index;
        public bool selected;

        public event Action<int> OnButtonPressed = delegate { };
        #endregion

        public void Initialize(int index, bool selected)
        {
            this.index = index;
            this.selected = selected;
        }

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnButtonPressed(index));
        }

        public void RegisterListener(Action<int> listener) => OnButtonPressed += listener;
        public void DeregisterListener(Action<int> listener) => OnButtonPressed -= listener;
    }
}