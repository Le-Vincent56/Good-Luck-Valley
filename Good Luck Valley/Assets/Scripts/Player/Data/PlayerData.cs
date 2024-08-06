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
        [SerializeField] public float movementTurnTime;
        [SerializeField] public float crawlSpeed;

        [Header("Sliding")]
        [SerializeField] public float maxFastSlideScalar;
        [SerializeField] public float slideAcceleration;

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

        [Header("Bounce")]
        [SerializeField] public float secondBounceMult;
        [SerializeField] public float thirdBounceMult;
    }
}