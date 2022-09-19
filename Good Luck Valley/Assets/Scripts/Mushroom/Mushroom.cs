using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Rigidbody2D mushroomRigidbody;

 
    /// <summary>
    /// Adding an initial force to the mushroom when thrown.
    /// </summary>
    /// <param name="mushroom"></param>
    public void AddForce(GameObject mushroom)
    {
        mushroom.GetComponent<Rigidbody2D>().AddForce(new Vector2(10, 10), ForceMode2D.Impulse);
    }
}
