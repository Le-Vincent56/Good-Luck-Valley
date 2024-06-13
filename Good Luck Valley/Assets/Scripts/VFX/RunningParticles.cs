using GoodLuckValley.Player.Control;
using GoodLuckValley.World.Tiles;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class RunningParticles : MonoBehaviour
    {
        // Fields
        [Header("Particle Systems")]
        [SerializeField] private VisualEffect grassParticle;
        [SerializeField] private VisualEffect dirtParticle;

        [Header("Particle Information")]
        [SerializeField] private float timeSinceLastParticle;
        [SerializeField] private float timeBetweenParticles;
        [SerializeField] private float minXVelocity;

        private PlayerController playerController;
        [SerializeField] private DetectTileType detectTileType;

        private void Awake()
        {
            playerController = GetComponentInParent<PlayerController>();
            detectTileType.RaycastStart = transform.position;
        }

        private void Update()
        {
            // Need a way to check if player is grounded
            if (timeSinceLastParticle < timeBetweenParticles)
            {
                timeSinceLastParticle += Time.deltaTime;
            }
            else if (playerController.IsGrounded && Mathf.Abs(playerController.Velocity.x) > minXVelocity)
            {
                switch (detectTileType.CheckTileType())
                {
                    case TileType.Dirt:
                        PlayEffect(dirtParticle);
                        break;
                    case TileType.Grass:
                        PlayEffect(grassParticle);
                        break;
                    case TileType.None:
                        Debug.Log("no tile type detected");
                        break;
                }
                timeSinceLastParticle = 0;
            }
        }

        public void PlayEffect(VisualEffect vfx)
        {
            vfx.Play();
        }
    }
}