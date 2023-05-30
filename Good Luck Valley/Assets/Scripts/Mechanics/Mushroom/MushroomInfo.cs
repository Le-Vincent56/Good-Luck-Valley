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
    private bool onScreen;
    [SerializeField] private bool isShroom;
    float pointDistance;
    [SerializeField]public Vector2 insidePoint;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    public bool IsShroom { get { return isShroom; } set { isShroom = value; } }
    #endregion

    private void Start()
    {
        pointDistance = GetComponent<CircleCollider2D>().radius;
        insidePoint = GetComponent<CircleCollider2D>().bounds.center;
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
    }
}
