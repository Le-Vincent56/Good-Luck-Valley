using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomInfo : MonoBehaviour
{
    public bool hasRotated;
    public float sT;
    public float timeStep = 0.01f;
    public bool bouncing = false;
    public float bouncingTimer = 0.1f;
    public GameObject mushroom;

    // Update is called once per frame
    void Update()
    {
        if (bouncing)
        {
            bouncingTimer -= Time.deltaTime;
            if (bouncingTimer <= 0)
            {
                bouncing = false;
                GetComponent<Animator>().SetBool("Bouncing", false);
            }
        }
    }
}
