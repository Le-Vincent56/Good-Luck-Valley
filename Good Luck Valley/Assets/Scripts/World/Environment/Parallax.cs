using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private CameraScriptableObj cameraEvent;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    #endregion

    #region FIELDS
    [SerializeField] private float parallaxScrolling;

    [Header("0: no parallax | 1: parallax speed = anari speed")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxMultiplyValue;
    #endregion

    private void OnEnable()
    {
        cameraEvent.moveEvent.AddListener(UpdateParallax);
    }

    private void OnDisable()
    {
        cameraEvent.moveEvent.RemoveListener(UpdateParallax);
    }

    private void UpdateParallax()
    {
        // Set the parallax scrolling
        parallaxScrolling = (movementEvent.GetMovementVector().x * parallaxMultiplyValue);

        if (disableEvent.GetDisableParallax() == false)
        {
            transform.position = new Vector3(transform.position.x + (parallaxScrolling * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }
}
