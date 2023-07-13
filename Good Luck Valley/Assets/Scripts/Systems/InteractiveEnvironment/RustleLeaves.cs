using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RustleLeaves : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] VisualEffect leavesParticle;
    #endregion

    #region FIELDS
    #endregion

    #region PROPERTIES
    #endregion

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Mushroom")
        {
            leavesParticle.Play();
        }
    }
}
