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
    [SerializeField] Vector3 birdDefaultAngle;
    [SerializeField] Vector3 birdFlapAngle;
    [SerializeField] float birdDuration;
    #endregion

    #region FIELDS
    private bool birdsPlayed = false;
    #endregion

    #region PROPERTIES
    #endregion

    private void Start()
    {
        birdsPlayed = false;
        birdDefaultAngle = birdsParticle.GetVector3(Shader.PropertyToID("BirdAngle"));
        if (birdDuration == 0)
        {
            int durationID = Shader.PropertyToID("RandomLifetimeB");
            birdDuration = birdsParticle.GetFloat(durationID);
        }
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
        // Vector3 birdAngle = birdsParticle.GetVector3(Shader.PropertyToID("BirdAngle"));
        // int counter = 0;
        while (elapsedTime < birdDuration)
        {
            // Testing out making birds flap, required?
            //if (birdAngle == birdDefaultAngle && counter == 3)
            //{
            //    Debug.Log("changing to flap: " + birdFlapAngle);
            //    birdAngle = birdFlapAngle;
            //}
            //else if (birdAngle == birdFlapAngle && counter == 6)
            //{
            //    Debug.Log("changing to default: " + birdDefaultAngle);
            //    birdAngle = birdDefaultAngle;
            //    counter = 0;
            //}
            //counter++;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        birdsParticle.Stop();
    }
}
