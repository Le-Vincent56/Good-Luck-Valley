using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    #region REFERENCES
    private GrassVelocityController grassVelocityController;

    private GameObject player;

    private Material material;

    private Rigidbody2D playerRB;

    private int externalInfluence;
    #endregion

    #region FIELDS
    private bool easeInRunning;
    private bool easeOutRunning;

    private float startingXVelocity;
    private float velocityLastFrame;
    #endregion

    private void Start()
    {   
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        grassVelocityController = GetComponentInParent<GrassVelocityController>();
        externalInfluence = grassVelocityController.ExternalInfluence;

        material = GetComponent<SpriteRenderer>().material;
        startingXVelocity = material.GetFloat(externalInfluence);
        Debug.Log("stating vel: " + startingXVelocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!easeInRunning && Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                Debug.Log("Enter");
                StartCoroutine(EaseIn(playerRB.velocity.x * grassVelocityController.ExternalInfluenceStrength));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Exit");
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!easeInRunning && Mathf.Abs(velocityLastFrame) > Mathf.Abs(grassVelocityController.VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) < Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            }
            else if (!easeOutRunning && Mathf.Abs(velocityLastFrame) < Mathf.Abs(grassVelocityController.VelocityThreshold) &&
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerRB.velocity.x * grassVelocityController.ExternalInfluenceStrength));
            }
            else if (!easeOutRunning && !easeInRunning && 
                Mathf.Abs(playerRB.velocity.x) > Mathf.Abs(grassVelocityController.VelocityThreshold))
            {
                Debug.Log("SHOULDNT HAPPEN");
                //grassVelocityController.InfluenceGrass(material, playerRB.velocity.x * grassVelocityController.ExternalInfluenceStrength);
            }

            velocityLastFrame = playerRB.velocity.x;
        }
    }

    private IEnumerator EaseIn(float xVelocity)
    {
        easeInRunning = true;

        Debug.Log("Starting Velocity: " + startingXVelocity);

        Debug.Log("Velocity: " + xVelocity);

        float elapsedTime = 0f;
        while (elapsedTime < grassVelocityController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.Lerp(startingXVelocity, xVelocity, (elapsedTime / grassVelocityController.EaseInTime));
            grassVelocityController.InfluenceGrass(material, lerpAmount);

            yield return null;
        }

        easeInRunning = false;  
    }

    private IEnumerator EaseOut()
    {
        easeOutRunning = true;

        float currentXInfluence = material.GetFloat(externalInfluence);

        float elapsedTime = 0f;
        while (elapsedTime < grassVelocityController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elapsedTime / grassVelocityController.EaseOutTime));
            grassVelocityController.InfluenceGrass(material, lerpAmount);

            Debug.Log("External Influence: " + currentXInfluence);

            yield return null;
        }

        easeOutRunning = false;
    }
}
