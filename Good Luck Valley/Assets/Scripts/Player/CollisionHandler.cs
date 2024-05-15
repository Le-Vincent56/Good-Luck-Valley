using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    #region EVENTS
    [SerializeField] private GameEvent onShroomEnter;
    #endregion

    private void TriggerBounce(GameObject mushroom)
    {
        // Apply shroom bounce
        onShroomEnter.Raise(this, mushroom);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for the correct game object
        if (collision.gameObject.tag != "Mushroom") TriggerBounce(collision.gameObject);
    }
}
