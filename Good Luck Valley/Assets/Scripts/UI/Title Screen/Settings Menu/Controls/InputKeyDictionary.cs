using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls
{
    [CreateAssetMenu(fileName = "InputKeyBindings", menuName = "Input/Key Bindings Map")]
    public class InputKeyDictionary : ScriptableObject
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
            { "Left Alt", 52 },
            { "Right Alt", 53 },
            { "Left Shift", 54 },
            { "Right Shift", 55 },
            { "Left Control", 56 },
            { "Right Control", 57 },
            { "Escape", 58 },

            // Large keys
            { "Space", 59 },

            // Mouse
            { "LMB", 60 },
            { "MMB", 61 },
            { "RMB", 62 }
        };

        public Sprite GetKey(string keyName) => KeyImages[Keys[keyName]];
    }
}