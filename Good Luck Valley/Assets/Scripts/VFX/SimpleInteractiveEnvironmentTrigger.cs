using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class SimpleInteractiveEnvironmentTrigger : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private bool hasLight;
    [SerializeField] private bool hasParticle;
    [SerializeField] private float totalReactiveTime;
    [SerializeField] private float currentReactiveTime;
    #endregion

    #region REFERENCES
    [SerializeField] private Light2D sparkleLight;
    [SerializeField] private VisualEffect vfx;
    #endregion


}