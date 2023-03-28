using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomInfo : MonoBehaviour
{
    #region REFERENCES

    #endregion

    #region FIELDS
    private bool hasRotated;
    private float sT;
    private float timeStep = 0.01f;
    private bool bouncing = false;
    private float bouncingTimer = 0.1f;
    private bool onScreen;
    [SerializeField] private bool isShroom;
    public List<Vector2> colliderPoints;
    float pointDistance;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    public bool IsShroom { get { return isShroom; } set { isShroom = value; } }
    #endregion

    private void Start()
    {
        pointDistance = GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        colliderPoints.Clear();
        colliderPoints.Add(new Vector2(transform.position.x, transform.position.y + pointDistance));                  // top
        colliderPoints.Add(new Vector2(transform.position.x, transform.position.y - pointDistance));                  // bottom
        colliderPoints.Add(new Vector2(transform.position.x + pointDistance, transform.position.y));                  // right
        colliderPoints.Add(new Vector2(transform.position.x - pointDistance, transform.position.y));                  // left

        colliderPoints.Add(new Vector2(transform.position.x + pointDistance * Mathf.Cos(45), transform.position.y + pointDistance * Mathf.Sin(45)));  // top right
        colliderPoints.Add(new Vector2(transform.position.x - pointDistance * Mathf.Cos(45), transform.position.y + pointDistance * Mathf.Sin(45)));  // top left
        colliderPoints.Add(new Vector2(transform.position.x + pointDistance * Mathf.Cos(45), transform.position.y - pointDistance * Mathf.Sin(45)));  // bottom right
        colliderPoints.Add(new Vector2(transform.position.x - pointDistance * Mathf.Cos(45), transform.position.y - pointDistance * Mathf.Sin(45)));  // bottom left

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
