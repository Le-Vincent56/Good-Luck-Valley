using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class BounceParticles : MonoBehaviour
    {
        // References
        [SerializeField] private VisualEffect shroomBounceParticles;

        public void PlayShroomBounceParticles(Component sender, object data)
        {
            if (data is not MushroomBounce.BounceData) return;

            shroomBounceParticles.SetVector2("SpawnPosition", (Vector2)transform.position);
            shroomBounceParticles.Play();
        }
    }
}