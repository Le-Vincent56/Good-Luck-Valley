using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class BobUpDown : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private float bobMaxSpeed;
    [SerializeField] private float bobMinSpeed;
    [SerializeField] private float maxBobHeightDifference;
    [SerializeField] private bool bobbingUp;
    [SerializeField] private float bobCurrentSpeed;
    [SerializeField] private float upperHeightPercent;
    [SerializeField] private float lowerHeightPercent;
    private float bobStartingHeight;
    private float bobEndingHeight;
    private float highPercentHeight;
    private float lowPercentHeight;
    #endregion

    #region REFERENCES
    #endregion

    private void Awake()
    {
        bobStartingHeight = transform.position.y;
        bobEndingHeight = maxBobHeightDifference + bobStartingHeight;
        bobCurrentSpeed = bobMaxSpeed;
        highPercentHeight = (upperHeightPercent * maxBobHeightDifference) + bobStartingHeight;
        lowPercentHeight = (lowerHeightPercent * maxBobHeightDifference) + bobStartingHeight;
    }

    private void Update()
    {
        if (bobCurrentSpeed > bobMaxSpeed)
            bobCurrentSpeed = bobMaxSpeed;
        if (bobCurrentSpeed < bobMinSpeed)
            bobCurrentSpeed = bobMinSpeed;

        float t = Time.deltaTime * bobCurrentSpeed;

        Vector2 position = transform.position;

        if (position.y <= bobStartingHeight)
        {
            bobbingUp = true;
        }
        else if (position.y >= bobEndingHeight)
        {
            bobbingUp = false;
        }

        if (bobbingUp) 
        {
            //Debug.Log("Y Position: " + position.y);
            position.y += Mathf.Lerp(0, maxBobHeightDifference, t);
            CalculateSpeedDampening(position);
        }
        else
        {
            position.y -= Mathf.Lerp(0, maxBobHeightDifference, t);
            CalculateSpeedDampening(position);
        }

        transform.position = position;
    }

    private void DampenSpeed(bool slowDown, float positionPercent)
    {
        //Debug.Log("Damping");
        if (slowDown && bobCurrentSpeed > bobMinSpeed)
        {
            Debug.Log("Slow down speed percent: " + positionPercent);
            bobCurrentSpeed = (1 - positionPercent) * bobMaxSpeed;
        }
        else if (!slowDown && bobCurrentSpeed < bobMaxSpeed)
        {
            Debug.Log("Speed speed percent: " + positionPercent);
            bobCurrentSpeed = (1 - positionPercent) * bobMaxSpeed;
        }
    }

    private void CalculateSpeedDampening(Vector2 position)
    {
        if (position.y > highPercentHeight)
        {
            float positionBasedSpeedPercent = (position.y - highPercentHeight) / (bobEndingHeight - highPercentHeight);
            if (bobbingUp)
                DampenSpeed(true, positionBasedSpeedPercent);
            else
                DampenSpeed(false, positionBasedSpeedPercent);
        }
        else if (position.y < lowPercentHeight)
        {
            float positionBasedSpeedPercent = (position.y - lowPercentHeight) / (bobEndingHeight - lowPercentHeight);
            if (bobbingUp)
                DampenSpeed(false, positionBasedSpeedPercent);
            else
                DampenSpeed(true, positionBasedSpeedPercent);
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(new Vector2(transform.position.x, ((0.8f * maxBobHeightDifference) + bobStartingHeight)), 0.1f);
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(new Vector2(transform.position.x, (bobStartingHeight)), 0.1f);
    }
}
