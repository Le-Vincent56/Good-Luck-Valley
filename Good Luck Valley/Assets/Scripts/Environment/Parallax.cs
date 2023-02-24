using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPosition;
    private GameObject mainCam;
    [SerializeField] private float parallaxScrolling;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        startPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (mainCam.transform.position.x * parallaxScrolling);
        transform.position = new Vector3(startPosition - distance, transform.position.y, transform.position.z);
    }
}
