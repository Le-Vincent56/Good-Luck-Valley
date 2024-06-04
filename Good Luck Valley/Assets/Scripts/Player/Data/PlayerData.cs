using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GHoodLuckValley.Player.Data
{
    [CreateAssetMenu(fileName = "PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Physics")]
        [SerializeField] public float accelerationTimeGround;
        [SerializeField] public float accelerationTimeAir;

        [Header("Falling")]
        [SerializeField] public float maxFallSpeed;
        [SerializeField] public float fastFallScalar;
        [SerializeField] public float maxFastFallSpeed;

        [Header("Movement")]
        [SerializeField] public float movementSpeed;
        [SerializeField] public float fastSlopeScalar;

        [Header("Jump")]
        [SerializeField] public float maxJumpHeight;
        [SerializeField] public float minJumpHeight;
        [SerializeField] public float timeToJumpApex;
        [SerializeField] public float coyoteTime;
        [SerializeField] public float jumpBufferTime;

        [Header("Walls")]
        [SerializeField] public float maxWallSlideSpeed;
        [SerializeField] public float fastWallSlideScalar;
        [SerializeField] public float maxFastWallSlideSpeed;
        [SerializeField] public float wallStickTime;
    }
}