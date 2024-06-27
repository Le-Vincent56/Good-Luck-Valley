using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomBounceEffect : MonoBehaviour
    {
        public void ApplyEffect(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not MushroomBounce.BounceData) return;

            // Cast data
            MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;

            switch(bounceData.BounceCount)
            {
                case 1:
                    transform.localScale = new Vector3(0.75f, 0.75f);
                    break;

                case 2:
                    transform.localScale = new Vector3(1.0f, 1.0f);
                    break;

                case 3:
                    transform.localScale = new Vector3(0.5f, 0.5f);
                    break;
            }
        }
    }
}