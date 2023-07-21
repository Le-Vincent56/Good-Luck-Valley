using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region REFERENCES
    private GameObject mainCam;
    [SerializeField] private MovementScriptableObj movementEvent;
    #endregion

    #region FIELDS
    [SerializeField] private float parallaxScrolling;

    [Header("0: no parallax | 1: parallax speed = anari speed")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxMultiplyValue;
    private Vector2 previousPos;
    private Vector2 currentPos;
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
        Debug.Log("Player Vel: " + movementEvent.GetMovementVector());
        Debug.Log("Parallax Speed: " + parallaxScrolling);

        float currentX = Mathf.Round(currentPos.x * 100f) / 100f;
        float prevX = Mathf.Round(previousPos.x * 100f) / 100f;

        if (currentX != prevX)
        {
            transform.position = new Vector3(transform.position.x + (parallaxScrolling * Time.deltaTime), transform.position.y, transform.position.z);
        }

        previousPos = currentPos;
    }
}
