using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.VFX;

namespace GoodLuckValley.Mushroom
{
    public class MushroomBounceEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffect shroomBounceParticles;

        public void ApplyEffect(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not MushroomBounce.BounceData) return;

            // Cast data
            MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;

            // Return if a mismatching index is sent
            if (bounceData.Index != GetComponent<MushroomInfo>().Index) return;

            switch(bounceData.BounceCount)
            {
                case 1:
                    //transform.localScale = new Vector3(0.75f, 0.75f);
                    PlayBounceParticles(bounceData, 1);
                    break;

                case 2:
                    //transform.localScale = new Vector3(1.0f, 1.0f);
                    PlayBounceParticles(bounceData, 2);
                    break;

                case 3:
                    //transform.localScale = new Vector3(0.5f, 0.5f);
                    PlayBounceParticles(bounceData, 3);
                    break;

                default:
                    //transform.localScale = new Vector3(0.5f, 0.5f);
                    PlayBounceParticles(bounceData, 3);
                    break;
            }
        }

        private void PlayBounceParticles(MushroomBounce.BounceData bounceData, float effectScalar)
        {
            shroomBounceParticles.SetFloat("EffectScalar", effectScalar);
            shroomBounceParticles.SetVector2("SpawnPosition", transform.position);
            shroomBounceParticles.Play();
        }
    }
}