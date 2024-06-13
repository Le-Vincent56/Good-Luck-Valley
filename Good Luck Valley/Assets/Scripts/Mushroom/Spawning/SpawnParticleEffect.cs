using GoodLuckValley.World.Tiles;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.Mushroom
{
    public class SpawnParticleEffect : MonoBehaviour
    {
        // Fields
        [SerializeField] private DetectTileType detectTileType;

        [Header("Particle Systems")]
        [SerializeField] private VisualEffect grassParticle;
        [SerializeField] private VisualEffect dirtParticle;

        private void Awake()
        {
            detectTileType.RaycastStart = transform.position;

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
        }

        public void PlayEffect(VisualEffect vfx)
        {
            vfx.Play();
        }
    }
}