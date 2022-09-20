using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D mushroomRigidbody;

 
    /// <summary>
    /// Adds an initial velocity to a mushroom, launching it in the direction of the mouse
    /// </summary>
    /// <param name="mushroom"> the mushroom object to be launched </param>
    /// <param name="dir"> the direction to launch in, either 1 for right or -1 for left </param>
    public void AddForce(GameObject mushroom, int dir)
    {
        mushroom.GetComponent<Rigidbody2D>().AddForce(new Vector2(10 * dir, 10), ForceMode2D.Impulse);
    }
}
