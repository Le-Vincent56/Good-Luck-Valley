using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class InteractiveBush : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] VisualEffect leavesParticle;
    private Material mat;
    private int squashRef;
    private float minRustleValue;
    #endregion

    #region FIELDS
    [SerializeField] private float rustleDuration = 0.35f;
    [SerializeField] private bool rustling;
    [SerializeField] private float maxRustleValue = 1.1f;
    [SerializeField] private bool playedSound = false;
    #endregion

    #region PROPERTIES
    #endregion

    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        squashRef = Shader.PropertyToID("_Squash");
        minRustleValue = mat.GetFloat(squashRef);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !rustling)
        {
            // If a sound hasn't been played, play one
            if(!playedSound)
            {
                AudioManager.Instance.PlayRandomizedOneShot(FMODEvents.Instance.BushRustles, transform.position);
                playedSound = true;
            }

            if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.x > 0)
            {
                mat.SetFloat("_SwapDirection", 1);
            }
            else
            {
                mat.SetFloat("_SwapDirection", 0);
            }
            StartCoroutine(Rustle());
        }

        if (collision.tag == "Player" || collision.tag == "Mushroom")
        {
            leavesParticle.transform.position = GetComponent<BoxCollider2D>().ClosestPoint(collision.transform.position);
            leavesParticle.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            // Set played sound to false to be able to trigger a sound again
            if(playedSound)
            {
                playedSound = false;
            }
        }
    }

    private IEnumerator Rustle()
    {
        rustling = true;

        float elapsedTime = 0;
        while (elapsedTime < rustleDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.Lerp(minRustleValue, maxRustleValue, (elapsedTime / rustleDuration));
            mat.SetFloat(squashRef, lerpAmount);
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < rustleDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpAmount = Mathf.Lerp(maxRustleValue, minRustleValue, (elapsedTime / rustleDuration));
            mat.SetFloat(squashRef, lerpAmount);
            yield return null;
        }

        rustling = false;
    }
}
