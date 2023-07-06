using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MushroomIndicator : MonoBehaviour
{
    #region REFERENCES
    private Camera cam;
    private GameObject cmCam;
    [SerializeField] GameObject mushroomToIndicate;
    private GameObject player;
    #endregion

    #region FIELDS
    [SerializeField] bool linkedToMushroom = false;
    private float camHeight;
    private float cmHeight;
    private float heightOffset;
    private float heightDampen;
    private float camWidth;
    private float cmWidth;
    private float widthOffset;
    private float widthDampen;
    private Vector2 indicatorPosition;
    #endregion

    #region PROPERTIES
    public GameObject MushroomToIndicate { get { return mushroomToIndicate; } set { mushroomToIndicate = value; } }
    public bool LinkedToMushroom { get { return linkedToMushroom; } set { linkedToMushroom = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cmCam = GameObject.Find("CM vcam1");
        player = GameObject.Find("Player");

        // Set to invisible
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        // Get camera bounds
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    // Update is called once per frame
    void Update()
    {

        // Check if the mushroomToIndicate is on screen, if so, enable the
        // Indicator's Renderer, disable it if the mushroomToIndicate is not on screen
        if (mushroomToIndicate != null && mushroomToIndicate.GetComponent<Shroom>().OnScreen)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        else if(mushroomToIndicate != null && !mushroomToIndicate.GetComponent<Shroom>().OnScreen)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        } else if(mushroomToIndicate == null)
        {
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // If the indicator is currently linked to a mushroom, track it
        if (linkedToMushroom)
        {
            if (mushroomToIndicate != null && mushroomToIndicate.GetComponent<Shroom>().IsShroom && !mushroomToIndicate.GetComponent<Shroom>().OnScreen)
            {
                // Find the direction from the screen center to the mushroom position
                Vector2 directionToShroom = (mushroomToIndicate.transform.position - transform.position).normalized;

                // Finding the angle in which the arrow should point
                float angle = Mathf.Atan2(directionToShroom.y, directionToShroom.x) * Mathf.Rad2Deg;

                // Set the rotation of the indicator to point at the mushroom
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
