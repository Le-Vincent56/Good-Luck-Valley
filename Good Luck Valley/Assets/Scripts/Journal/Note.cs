using GoodLuckValley.Events;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class Note : MonoBehaviour, IInteractable
    {
        #region EVENTS
        [SerializeField] private GameEvent onAddNote;
        #endregion

        #region REFERENCES
        [SerializeField] private NoteData data;
        #endregion


        public void Interact()
        {
            data.Print();
        }
    }
}