using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushInteractivity : MonoBehaviour
{
    #region REFERENCES
    private BushInteractivityController bushInteractivityController;

    private GameObject player;

    private Material material;

    private Rigidbody2D playerRB;

    private int rotationRef;
    #endregion

    #region FIELDS
    private float startRotation;
    private float currentRotation;
    #endregion

    private void Start()
    {
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        bushInteractivityController = GetComponentInParent<BushInteractivityController>();
        rotationRef = bushInteractivityController.RotationRef;

        material = GetComponent<SpriteRenderer>().material;
        startRotation = material.GetFloat(rotationRef);
        currentRotation = bushInteractivityController.RotationDegee;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(Rotate());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

        }
    }

    private IEnumerator Rotate()
    {
        Debug.Log("Rotating");
        float rotationCount = 0f;
        currentRotation = bushInteractivityController.RotationDegee;
        while (rotationCount < bushInteractivityController.Rotations)
        {
            rotationCount++;
            //float lerpAmount = Mathf.Lerp(startingXVelocity, xVelocity, (elapsedTime / grassVelocityController.EaseInTime));
            bushInteractivityController.RotateBush(material, currentRotation);

            Debug.Log("Rotate: " + currentRotation);
            currentRotation = -currentRotation;
            yield return null;
        }
        currentRotation = startRotation;
        bushInteractivityController.RotateBush(material, currentRotation);
    }

    //private IEnumerator EaseOut()
    //{
    //    //easeOutRunning = true;

    //    //float currentXInfluence = material.GetFloat(externalInfluence);

    //    //float elapsedTime = 0f;
    //    //while (elapsedTime < grassVelocityController.EaseOutTime)
    //    //{
    //    //    elapsedTime += Time.deltaTime;

    //    //    float lerpAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elapsedTime / grassVelocityController.EaseOutTime));
    //    //    Debug.Log(lerpAmount);
    //    //    grassVelocityController.InfluenceGrass(material, lerpAmount);

    //    //    yield return null;
    //    //}

    //    //easeOutRunning = false;
    //}
}
