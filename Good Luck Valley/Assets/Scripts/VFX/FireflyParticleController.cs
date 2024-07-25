using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class FireflyParticleController : MonoBehaviour
{
    private VisualEffect fireflyVFX;
    private float animationFrame;
    [SerializeField] private float animationSpeed;

    private void Awake()
    {
        fireflyVFX = GetComponent<VisualEffect>();
        animationFrame = 0;
    }

    private void Start()
    {
        fireflyVFX.Play();
    }

    private void Update()
    {
        animationFrame += Time.deltaTime * animationSpeed;
        fireflyVFX.SetFloat("Animation Frame", animationFrame);

        if (animationFrame > 7.0f)
        {
            animationFrame = 0;
        }
    }
}
