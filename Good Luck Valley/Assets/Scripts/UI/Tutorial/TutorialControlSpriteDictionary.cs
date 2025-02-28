using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Tutorial
{
    [CreateAssetMenu(fileName = "Tutorial Controls Sprite Dictionary", menuName = "Tutorial/Controls Sprite Dictionary")]
    public class TutorialControlSpriteDictionary : SerializedScriptableObject
    {
        public List<Sprite> Sprites = new List<Sprite>();
        public Dictionary<string, int> Keys = new Dictionary<string, int>();

        public Sprite GetSprite(string keyName) => Sprites[Keys[keyName]];
    }
}
