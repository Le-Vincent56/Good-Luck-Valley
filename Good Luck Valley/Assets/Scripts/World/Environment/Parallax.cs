using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region REFERENCES
    private GameObject mainCam;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    #endregion

    #region FIELDS
    [SerializeField] private float parallaxScrolling;

    [Header("0: no parallax | 1: parallax speed = anari speed")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxMultiplyValue;
    [SerializeField] private Vector2 previousPos;
    [SerializeField] private Vector2 currentPos;
    [SerializeField] private float currentX;
    [SerializeField] private float prevX;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        currentPos = mainCam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = mainCam.transform.position;

        parallaxScrolling = (movementEvent.GetMovementVector().x * parallaxMultiplyValue);

        if (disableEvent.GetDisableParallax() == false)
        {
            transform.position = new Vector3(transform.position.x + (parallaxScrolling * Time.deltaTime), transform.position.y, transform.position.z);
        }

        previousPos = currentPos;
    }
}
