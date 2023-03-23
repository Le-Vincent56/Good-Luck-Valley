using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomInfo : MonoBehaviour
{
    #region REFERENCES
    public GameObject mushroom;
    #endregion

    #region FIELDS
    private bool hasRotated;
    private float sT;
    private float timeStep = 0.01f;
    private bool bouncing = false;
    private float bouncingTimer = 0.1f;
    private bool onScreen;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    #endregion


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
