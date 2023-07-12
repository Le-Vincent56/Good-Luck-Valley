using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RunningEffects : MonoBehaviour
{
    #region REFERENCES
    private PlayerMovement player;
    private GameObject effectsParent;
    private VisualEffect grassParticles;
    private VisualEffect dirtParticles;
    #endregion

    #region FIELDS
    #endregion

    #region PROPERTIES
    #endregion

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
