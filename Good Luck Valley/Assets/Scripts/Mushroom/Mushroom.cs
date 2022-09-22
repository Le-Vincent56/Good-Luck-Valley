using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D mushroomRigidbody;

    /// <summary>
    /// Adds an initial velocity to a mushroom, launching it in the direction of the mouse
    /// </summary>
    /// <param name="mushroom"></param>
    public void AddForce(GameObject mushroom, Vector2 mousePos)
    {
        mushroom.GetComponent<Rigidbody2D>().AddForce(new Vector2(mousePos.x, mousePos.y), ForceMode2D.Impulse);
    }
}
