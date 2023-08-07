using UnityEngine;
using UnityEngine.VFX;

namespace HiveMind.Particles
{
    public class RustleLeaves : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] VisualEffect leavesParticle;
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player" || collision.tag == "Mushroom")
            {
                leavesParticle.Play();
            }
        }
    }
}

