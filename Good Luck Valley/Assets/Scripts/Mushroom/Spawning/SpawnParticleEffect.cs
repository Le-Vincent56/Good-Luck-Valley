using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.VFX;


public class SpawnParticleEffect : MonoBehaviour
{
    // Fields
    private DetectTileType detectTileType;

    [SerializeField] private Transform raycastEndHardpoint;

    [Header("Particle Systems")]
    [SerializeField] private VisualEffect grassParticle;
    [SerializeField] private VisualEffect dirtParticle;



    private void Awake()
    {
        detectTileType = new DetectTileType(this.transform, raycastEndHardpoint);
        switch (detectTileType.CheckGroundTileAndPlayParticle())
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
