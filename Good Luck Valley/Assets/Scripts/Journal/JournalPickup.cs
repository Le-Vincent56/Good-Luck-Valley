using GoodLuckValley.Events;
using GoodLuckValley.World.Interactables;
using UnityEngine;

namespace GoodLuckValley.Journal.Collectibles
{
    public class JournalPickup : Collectible
    {
        [SerializeField] private GameEvent onTeachInteractable;

        public override void Interact()
        {
            base.Interact();

            // Unlock the journal
            Journal.Instance.Unlock();

            // Teach interactable
            onTeachInteractable.Raise(this, null);
        }
    }
}