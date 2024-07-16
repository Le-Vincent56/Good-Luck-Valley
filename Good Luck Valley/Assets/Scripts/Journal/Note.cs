using GoodLuckValley.Events;
using GoodLuckValley.World.Interactables;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class Note : Collectible
    {
        #region EVENTS
        [SerializeField] private GameEvent onAddNote;
        #endregion

        public override void Interact()
        {
            base.Interact();

            // Unlock the next journal note
            Journal.Instance.GetNextNote();
        }
    }
}