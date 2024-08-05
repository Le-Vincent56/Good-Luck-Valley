using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.VFX.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BurstParticleData", menuName = "VFXData/BurstParticleData")]
    public class VFXBurstParticleData : ScriptableObject
    {
        [Header("Spawning")]
        [SerializeField] public float spawnCount;

        [Header("Initializing")]
        [SerializeField] public float minXVelocity;
        [SerializeField] public float maxXVelocity;
        [SerializeField] public float minYVelocity;
        [SerializeField] public float maxYVelocity;
        [SerializeField] public float minLifetime;
        [SerializeField] public float maxLifetime;
        [SerializeField] public float minSize;
        [SerializeField] public float maxSize;

        [Header("Update")]
        [SerializeField] public float gravity;
        [SerializeField] public float linearDragCoefficient;
        [SerializeField] public float turbulenceIntensity;
        [SerializeField] public float turbulenceDrag;
        [SerializeField] public float turbulenceFrequency;
        [SerializeField] public float addZAngle;
        [SerializeField] public float minAddAngleScalar;
        [SerializeField] public float maxAddAngleScalar;

        [Header("Output")]
        [SerializeField] public Texture2D mainTex;
        [SerializeField] public AnimationCurve multiplySizeOverLife;
        [SerializeField] public Gradient colorOverLife;
    }
}
