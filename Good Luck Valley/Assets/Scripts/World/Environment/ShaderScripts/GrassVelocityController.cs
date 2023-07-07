using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassVelocityController : MonoBehaviour
{
    #region REFERENCES
    private int externalInfluence = Shader.PropertyToID("_ExternalInfluence");
    #endregion

    #region FIELDS
    [Range(0f, 1f)] private float externalInfluenceStrength = .25f;
    private float easeInTime = .15f;
    private float easeOutTime = .15f;
    private float velocityThreshold = 2f;
    #endregion

    #region PROPERTIES
    public float ExternalInfluenceStrength {  get { return externalInfluenceStrength; } set { externalInfluenceStrength = value; } }
    public float EaseInTime { get {  return easeInTime; } set {  easeInTime = value; } }
    public float EaseOutTime { get { return easeOutTime; } set { easeOutTime = value; } }
    public float VelocityThreshold { get { return velocityThreshold; } set { velocityThreshold = value; } }
    public int ExternalInfluence { get { return externalInfluence; } }
    #endregion

    public void InfluenceGrass(Material mat, float xVelocity)
    {
        mat.SetFloat(externalInfluence, xVelocity);
    }
}
