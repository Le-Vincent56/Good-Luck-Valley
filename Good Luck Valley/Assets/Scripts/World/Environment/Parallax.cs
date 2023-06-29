using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region REFERENCES
    private GameObject mainCam;
    #endregion

    #region FIELDS
    [SerializeField] private float parallaxScrolling;
    private float startPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        startPosition = transform.position.x;
        parallaxScrolling /= 10f;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (mainCam.transform.position.x * parallaxScrolling);

        if (distance == 0)
        {
            transform.position = new Vector3(mainCam.transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(startPosition - distance, transform.position.y, transform.position.z);
        }
    }
}
