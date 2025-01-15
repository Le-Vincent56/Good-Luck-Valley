using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Input
{
    [CreateAssetMenu(fileName = "Input Key Bindings", menuName = "Input/Key Bindings Map")]
    public class InputKeyDictionary : SerializedScriptableObject
    {
        public List<Sprite> KeyImages = new List<Sprite>();
        public Dictionary<string, int> Keys = new Dictionary<string, int>()
        {
            // Small Keys
            { "0", 0 },
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
            { "A", 10 },
            { "B", 11 },
            { "C", 12 },
            { "D", 13 },
            { "E", 14 },
            { "F", 15 },
            { "G", 16 },
            { "H", 17 },
            { "I", 18 },
            { "J", 19 },
            { "K", 20 },
            { "L", 21 },
            { "M", 22 },
            { "N", 23 },
            { "O", 24 },
            { "P", 25 },
            { "Q", 26 },
            { "R", 27 },
            { "S", 28 },
            { "T", 29 },
            { "U", 30 },
            { "V", 31 },
            { "W", 32 },
            { "X", 33 },
            { "Y", 34 },
            { "Z", 35 },
            { "`", 36 },
            { "'", 37 },
            { "\\", 38 },
            { "/", 39 },
            { ".", 40 },
            { ",", 41 },
            { "-", 42 },
            { "=", 43 },
            { ";", 44 },
            { "[", 45 },
            { "]", 46 },
            { "Up Arrow", 47 },
            { "Down Arrow", 48 },
            { "Left Arrow", 49 },
            { "Right Arrow", 50 },

            // Medium Keys
            { "Tab", 51},
            { "Enter", 52 },
            { "Left Alt", 53 },
            { "Right Alt", 54 },
            { "Alt", 55 },
            { "Left Shift", 56 },
            { "Right Shift", 57 },
            { "Shift", 58 },
            { "Left Control", 59 },
            { "Right Control", 60 },
            { "Control", 61 },
            { "Escape", 62 },

            // Large keys
            { "Space", 63 },

            // Mouse
            { "LMB", 64 },
            { "MMB", 65 },
            { "RMB", 66 },
            { "Forward", 67 },
            { "Back", 68 }
        };

        public Sprite GetKey(string keyName) => KeyImages[Keys[keyName]];
    }
}
