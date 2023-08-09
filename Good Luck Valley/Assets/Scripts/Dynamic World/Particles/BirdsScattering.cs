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
            birdsParticle.enabled = true;
            birdsParticle.Play();
            StartCoroutine(BirdsTimer());
            birdsPlayed = true;
        }
    }

    private IEnumerator BirdsTimer()
    {
        float elapsedTime = 0f;
        int durationID = Shader.PropertyToID("RandomLifetimeB");
        while (elapsedTime < birdsParticle.GetFloat(durationID))
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        birdsParticle.Stop();
    }
}
