using HiveMind.Events;
using HiveMind.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class BirdsScattering : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private VisualEffect birdsParticle;
    #endregion

    #region FIELDS
    private bool birdsPlayed = false;
    private bool birdsPlaying = false;
    #endregion

    #region PROPERTIES
    #endregion

    private void Start()
    {
        birdsPlayed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !birdsPlayed)
        {
            birdsParticle.Play();
            birdsPlaying = true;
            birdsPlayed = true;
        }
    }
}
