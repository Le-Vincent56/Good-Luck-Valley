using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushInteractivityController : MonoBehaviour
{
    #region REFERENCES
    private int rotationRef = Shader.PropertyToID("_Rotation");
    #endregion

    #region FIELDS
    [SerializeField] private int rotations;
    [SerializeField] private int rotationDegee;
    #endregion

    #region PROPERTIES
    public int RotationRef { get { return rotationRef; } }
    public int Rotations { get { return rotations; } }
    public int RotationDegee { get {  return rotationDegee; } }
    #endregion

    public void RotateBush(Material mat, float rotation)
    {
        mat.SetFloat(rotationRef, rotation);
    }
}
