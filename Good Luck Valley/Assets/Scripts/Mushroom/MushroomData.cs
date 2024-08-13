using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mushroom Data")]
public class MushroomData : ScriptableObject
{
    [SerializeField] public float regularBounceForce;
    [SerializeField] public float regularSlopeBounceForce;
    [SerializeField] public float wallBounceForce;
}
