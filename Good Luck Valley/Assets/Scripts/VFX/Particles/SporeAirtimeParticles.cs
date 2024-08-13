using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX.Particles
{
    public class SporeAirtimeParticles : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;

        private void Awake()
        {
            // Get VFX
            vfx = Instantiate(vfx);
            vfx.Play();
        }

        private void Update()
        {
            vfx.SetVector2("SpawnPosition", transform.position);
        }

        private void OnDisable()
        {
            vfx.Stop();
        }
    }
}