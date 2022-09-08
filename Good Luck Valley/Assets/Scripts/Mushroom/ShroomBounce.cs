using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShroomBounce : MonoBehaviour
{
    #region FIELDS
    [SerializeField] float minimumBounceDistance;
    float currentMaxDistance;
    #endregion

    #region VARIABLES
    public Rigidbody2D RB { get; private set; }
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
