using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    #region EVENTS
    [Header("Events")]
    [SerializeField] private GameEvent onShroomEnter;
    #endregion

    private void TriggerBounce(MushroomData mushroom)
    {
        // Apply shroom bounce
        // Calls to:
        //  - MushroomBounce.Bounce()
        onShroomEnter.Raise(this, mushroom);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for the correct game object
        if (collision.gameObject.tag == "Mushroom") TriggerBounce(collision.gameObject.GetComponent<MushroomData>());
    }
}
