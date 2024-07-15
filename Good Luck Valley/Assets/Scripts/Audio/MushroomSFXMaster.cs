using UnityEngine;
using AK.Wwise;
using GoodLuckValley.World.Tiles;

namespace GoodLuckValley.Audio.SFX
{
    public class MushroomSFXMaster : MonoBehaviour
    {
        [Header("Wwise Switches")]
        [SerializeField] private Switch grassSwitch;
        [SerializeField] private Switch dirtSwitch;
        [SerializeField] private Switch stoneSwitch;

        [Header("Mushroom Grow")]
        [SerializeField] private AK.Wwise.Event shroomGrowEvent;

        public void UpdateSwitch(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Dirt:
                    dirtSwitch.SetValue(gameObject);
                    break;

                case TileType.Grass:
                    grassSwitch.SetValue(gameObject);
                    break;

                case TileType.Stone:
                    stoneSwitch.SetValue(gameObject);
                    break;
            }
        }

        public void Grow() => shroomGrowEvent.Post(gameObject);
    }
}