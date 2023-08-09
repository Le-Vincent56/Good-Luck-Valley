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
        // Birds effect hasnt played
        birdsPlayed = false;

        // birdDefaultAngle = birdsParticle.GetVector3(Shader.PropertyToID("BirdAngle"));

        // If the duration is set to 0, hasn't been given a value in the inspector
        if (birdDuration == 0)
        {
            // Sets a default value to the duration using the lifetime of the birds
            int durationID = Shader.PropertyToID("RandomLifetimeB");
            birdDuration = birdsParticle.GetFloat(durationID);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the player is the collision entering and the birds effect has not played
        if (collision.CompareTag("Player") && !birdsPlayed)
        {
            // Enables the particle effect
            birdsParticle.enabled = true;

            // Plays the effect
            birdsParticle.Play();
            
            // Starts the timer to stop the effect
            StartCoroutine(BirdsTimer());

            // The birds effect has played
            birdsPlayed = true;
        }
    }
    /// <summary>
    /// A timer to stop the birds effect from playing after the given duration
    /// </summary>
    /// <returns></returns>
    private IEnumerator BirdsTimer()
    {
        // The current time
        float elapsedTime = 0f;

        // Testing out making birds flap winngs, required?
        // Vector3 birdAngle = birdsParticle.GetVector3(Shader.PropertyToID("BirdAngle"));
        // int counter = 0;

        // Loops while the current time is less than the duration of the effect
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

            // Increases the current time
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stops the birds after the timer is complete
        birdsParticle.Stop();
    }
}
