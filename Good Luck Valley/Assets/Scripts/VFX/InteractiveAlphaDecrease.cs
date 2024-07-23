using GoodLuckValley.World.AreaTriggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

[RequireComponent(typeof(AreaCollider))]

public class InteractiveAlphaDecrease : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private float currentAlphaChangeTime;
    [SerializeField] private float alphaChangeSpeed;
    private bool coroutineActive;
    #endregion

    #region REFERENCES
    private SpriteRenderer sprite;
    private AreaCollider areaCollider;
    #endregion

    private void Awake()
    {
        areaCollider = GetComponent<AreaCollider>();
        sprite = GetComponent<SpriteRenderer>();
        coroutineActive = false;
    }

    private void OnEnable()
    {
        areaCollider.OnTriggerEnter += StartAlphaChange;
    }
    private void OnDisable()
    {
        areaCollider.OnTriggerEnter -= StartAlphaChange;
    }


    private void StartAlphaChange(GameObject hitObject)
    {
        if (!coroutineActive)
        {
            coroutineActive = true;

            StartCoroutine(AlphaFadeOut());
        }
    }

    private IEnumerator AlphaFadeOut()
    {
        currentAlphaChangeTime = 0;
        while (sprite.color.a > 0.0f)
        {
            float spriteAlpha = sprite.color.a;
            currentAlphaChangeTime += alphaChangeSpeed * Time.deltaTime;

            sprite.color = new Color(sprite.color.r, 
                                     sprite.color.g, 
                                     sprite.color.b, 
                                     spriteAlpha -= Mathf.Lerp(0, 1, currentAlphaChangeTime));
            yield return null;
        }

        StartCoroutine(AlphaFadeIn());
    }

    private IEnumerator AlphaFadeIn()
    {
        currentAlphaChangeTime = 0;
        while (sprite.color.a < 1.0f)
        {
            float spriteAlpha = sprite.color.a;
            currentAlphaChangeTime += alphaChangeSpeed * Time.deltaTime;

            sprite.color = new Color(sprite.color.r,
                                     sprite.color.g,
                                     sprite.color.b,
                                     spriteAlpha += Mathf.Lerp(0, 1, currentAlphaChangeTime));
            yield return null;
        }

        coroutineActive = false;
    }
}
