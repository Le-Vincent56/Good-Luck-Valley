using GoodLuckValley.Events;
using GoodLuckValley.World.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class Note : Collectible
    {
        #region EVENTS
        [SerializeField] private GameEvent onAddNote;
        #endregion

        #region REFERENCES
        [SerializeField] private NoteData noteData;
        #endregion

        public override void Interact()
        {
            base.Interact();

            noteData.Print();
        }
    }
}