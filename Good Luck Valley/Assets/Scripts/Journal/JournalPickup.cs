using GoodLuckValley.World.Interactables;
using UnityEngine;

namespace GoodLuckValley.Journal.Collectibles
{
    public class JournalPickup : Collectible
    {
        public override void Interact()
        {
            base.Interact();

            // Unlock the journal
            Journal.Instance.Unlock();
        }
    }
}