using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatablePlatform : MoveablePlatform
{
    #region FIELDS
    [SerializeField] float angle;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 pivotPoint;
    Quaternion finalRotation;
    
    #endregion

    // Start is called before the first frame update
    void Start()
    { 
        finalRotation = Quaternion.Euler(0, 0, angle); // Rotate around the Z dimension
    }

    public override void Move() // Replaces "Move" in parent class (MoveablePlatform)
    {
        // Sets pivot point, direction and speed of rotation
        transform.RotateAround(pivotPoint, Vector3.forward, rotationSpeed * Time.deltaTime);

        

        if(transform.rotation == finalRotation)
        {
            isTriggered = false; // Stops rotation
        }

    }
}
