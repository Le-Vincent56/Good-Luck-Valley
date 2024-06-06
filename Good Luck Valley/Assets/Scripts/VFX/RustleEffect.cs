using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustleEffect : MonoBehaviour
{
    public LightWindVariableController shaderController;
    public bool testTrigger;
    public float testEM;
    public int testDir;
    public float testSpeed;
    public float startingMultiplier;
    public float currentMultiplier;
    public float endingMultiplier;
    public int direction;
    public float speed;


    private void Awake()
    {
        startingMultiplier = shaderController.directionMultiplier;
    }

    private void Update()
    {
        if (testTrigger)
        {
            InitiateRustle(testEM, testSpeed, testDir);
            testTrigger = false;
        }
    }

    public void InitiateRustle(float endingMultiplier, float speed, int direction)
    {
        Debug.Log("Initiating Rustle");
        this.endingMultiplier = endingMultiplier;
        this.direction = direction;
        this.speed = speed;
        currentMultiplier = startingMultiplier * direction;
        StartCoroutine(StartRustle());
    }

    private IEnumerator StartRustle()
    {
        while(Mathf.Abs(currentMultiplier) <= Mathf.Abs(endingMultiplier))
        {
            currentMultiplier += (speed * direction);
            shaderController.UpdateDirectionMultiplier(currentMultiplier);
            Debug.Log("Current Mult Start: " + currentMultiplier);
            yield return null;
        }
        Debug.Log("Ending Rustle");
        StartCoroutine(EndRustle());
        yield break;
    }

    private IEnumerator EndRustle()
    {
        while (Mathf.Abs(currentMultiplier) >= Mathf.Abs(startingMultiplier))
        {
            currentMultiplier -= (speed * direction);
            shaderController.UpdateDirectionMultiplier(currentMultiplier);
            Debug.Log("Current Mult End: " + currentMultiplier);
            yield return null;
        }
        yield break;
    }
}