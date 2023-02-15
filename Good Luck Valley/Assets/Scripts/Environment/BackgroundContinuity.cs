using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundContinuity : MonoBehaviour
{
    [SerializeField]
    float translateSpeed;
    [SerializeField]
    float upperBound;
    [SerializeField]
    float lowerBound;
    [SerializeField]
    PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player._isMoving)
        {

        }
    }
}
