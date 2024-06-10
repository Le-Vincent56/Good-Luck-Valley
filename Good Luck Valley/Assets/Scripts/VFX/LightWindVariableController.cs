using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightWindVariableController : MonoBehaviour
{
    [Header("FREE TO CHANGE")]
    public float windDirection;
    public float bendStrength;

    [Header("FOR RUSTLE EFFECT ONLY")]
    public float directionMultiplier;

    [Header("DO NOT CHANGE")]
    public float verticalOffset;

    private void Start()
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Bend_Strength", bendStrength);
        propertyBlock.SetFloat("_xDIrection", windDirection);
        propertyBlock.SetFloat("_Vertical_Offset", verticalOffset);
        propertyBlock.SetFloat("_DirectionMultiplier", directionMultiplier);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }

    public void UpdateDirectionMultiplier(float directionMultiplier)
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_DirectionMultiplier", directionMultiplier);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }

    public void UpdateBendStrength(float bendStrength)
    {
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Bend_Strength", bendStrength);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}