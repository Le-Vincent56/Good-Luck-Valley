using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Tutorial
{
    [CreateAssetMenu(fileName = "Tutorial Controls Animation Dictionary", menuName = "Tutorial/Controls Animation Dictionary")]
    public class TutorialControlsAnimationDictionary : SerializedScriptableObject
    {
        public List<RuntimeAnimatorController> Controllers = new List<RuntimeAnimatorController>();
        public Dictionary<string, int> Keys = new Dictionary<string, int>();

        public RuntimeAnimatorController GetController(string keyName) => Controllers[Keys[keyName]];
    }
}
