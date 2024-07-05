using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    [CreateAssetMenu(fileName = "Spore Data")]
    public class SporeData : ScriptableObject
    {
        [SerializeField] public float gravity;
    }
}