using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Mushroom.StateMachineOld;
using GoodLuckValley.World.Interactables;
using UnityEngine;

namespace GoodLuckValley.Player.Handlers
{
    public class CollisionHandlerOld : MonoBehaviour
    {
        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onShroomEnter;
        [SerializeField] private GameEvent setInteractable;
        #endregion

        private void TriggerBounce(MushroomData data)
        {
            // Apply shroom bounce
            // Calls to:
            //  - MushroomBounce.Bounce()
            onShroomEnter.Raise(this, data);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check for mushrooms
            if (collision.gameObject.tag == "Mushroom")
            {
                // Trigger a bounce
                TriggerBounce(collision.gameObject.GetComponent<MushroomData>());
                collision.gameObject.GetComponent<MushroomControllerOld>().SetIsBouncing(true);
            }

            // Check for interactables
            if (collision.gameObject.tag == "Interactable")
            {
                // Set no interactable
                // Calls to:
                //  - InteractableHandler.SetInteractable();
                setInteractable.Raise(this, collision.gameObject.GetComponent<IInteractable>());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Check for interactables
            if (collision.gameObject.tag == "Interactable")
            {
                // Set no interactable
                // Calls to:
                //  - InteractableHandler.SetInteractable();
                setInteractable.Raise(this, null);
            }
        }
    }
}