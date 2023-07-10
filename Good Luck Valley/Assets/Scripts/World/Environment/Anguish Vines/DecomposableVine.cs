using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecomposableVine : AnguishVine
{
    #region FIELDS
    [SerializeField] private float timeToDecompose = 1f;
    #endregion

    #region PROPERTIES
    public float DecomposeTime { get { return timeToDecompose; } set {  timeToDecompose = value; } }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
