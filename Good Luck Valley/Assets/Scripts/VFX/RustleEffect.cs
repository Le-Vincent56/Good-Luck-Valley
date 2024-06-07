using GoodLuckValley.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustleEffect : MonoBehaviour
{
    private AreaCollider areaCollider;

    public LightWindVariableController shaderController;
    public float startingMultiplier;
    public float currentMultiplier;
    public float endingMultiplier;
    public int direction;
    public float speed;
    private bool rustling;

    [Header("Testing Variables")]
    public bool testTrigger;
    public float testEM;
    public int testDir;
    public float testSpeed;


    private void Awake()
    {
        // Set references
        areaCollider = GetComponent<AreaCollider>();

        startingMultiplier = shaderController.directionMultiplier;
    }

    private void OnEnable()
    {
        areaCollider.OnTriggerEnter += Rustle;
    }

    private void OnDisable()
    {
        areaCollider.OnTriggerEnter -= Rustle;
    }

    //private void Update()
    //{
    //    if (testTrigger)
    //    {
    //        InitiateRustle(testEM, testSpeed, testDir);
    //        testTrigger = false;
    //    }
    //}

    private void Rustle(GameObject gameObject)
    {
        int enterDirection = Mathf.RoundToInt((gameObject.transform.position - areaCollider.Bounds.center).normalized.x);

        InitiateRustle(testEM, testSpeed, enterDirection);
    }

    /// <summary>
    /// Initiates the rustle effect
    /// </summary>
    /// <param name="endingMultiplier"> How intense the rustle will be, should depend on the speed of the object colliding </param>
    /// <param name="speed"> How fast the rustle will be, should depend on the speed of the object colliding </param>
    /// <param name="direction"> The "direction" of the rustle, either 1 or -1 </param>
    public void InitiateRustle(float endingMultiplier, float speed, int direction)
    {
        // Check if currently rustling
        if (!rustling) 
        {
            // Set values
            rustling = true;
            this.endingMultiplier = endingMultiplier;
            this.direction = direction;
            this.speed = speed;
            currentMultiplier = startingMultiplier * direction;

            // Start the rustle effect (moving away from starting position)
            StartCoroutine(StartRustle());
        }
    }

    private IEnumerator StartRustle()
    {
        // Loop while the current mult is still less than the ending mult goal
        while(Mathf.Abs(currentMultiplier) <= Mathf.Abs(endingMultiplier))
        {
            // Increase current mult
            currentMultiplier += (speed * direction);
            // Set current mult in the shader
            shaderController.UpdateDirectionMultiplier(currentMultiplier);
           
            yield return null;
        }

        // End the rustle effect (moving toward starting position)
        StartCoroutine(EndRustle());
        yield break;
    }

    private IEnumerator EndRustle()
    {
        // Loop while current mult is still greater than starting mult
        while (Mathf.Abs(currentMultiplier) >= Mathf.Abs(startingMultiplier))
        {
            // Decrease current mult
            currentMultiplier -= (speed * direction);
            
            // Set current mult in shader
            shaderController.UpdateDirectionMultiplier(currentMultiplier);
            yield return null;
        }

        // No longer ruslting
        rustling = false;
        yield break;
    }
}