using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomInfo : MonoBehaviour
{
    #region REFERENCES

    #endregion

    #region FIELDS
    private bool hasRotated;
    [SerializeField] float rotateAngle;
    private bool bouncing = false;
    private float bouncingTimer = 0.1f;
    [SerializeField] private float durationTimer;
    private bool onScreen;
    [SerializeField] private bool isShroom;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    public bool IsShroom { get { return isShroom; } set { isShroom = value; } }

    public float DurationTimer { get { return durationTimer; } }
    #endregion

    private void Awake()
    {
        durationTimer = 3f; // ~3 seconds
    }

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

        // Decreases deltaTime from timer for this shroom
        durationTimer -= Time.deltaTime;
    }
}
