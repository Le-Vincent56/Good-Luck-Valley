using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] GameObject levelLoader;

    #endregion

    #region FIELDS

    #endregion

    #region PROPERTIES

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.tag == "Player")
       {
            levelLoader.GetComponent<LoadLevel>().LoadNextLevel();
       }
        
    }
}
