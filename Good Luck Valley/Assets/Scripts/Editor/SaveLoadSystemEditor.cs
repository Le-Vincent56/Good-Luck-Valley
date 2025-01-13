using GoodLuckValley.Persistence;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors.Persistence
{
    [CustomEditor(typeof(SaveLoadSystem))]
    public class SaveLoadSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the target and the current game neame
            SaveLoadSystem saveLoadSystem = (SaveLoadSystem)target;
            string gameName = saveLoadSystem.GameData.Name;

            DrawDefaultInspector();

            // Debug button to create a new game
            if (GUILayout.Button("New Game"))
            {
                saveLoadSystem.NewGame(1);
            }

            // Debug button to save the game
            if (GUILayout.Button("Save Game"))
            {
                saveLoadSystem.SaveGame();
            }

            // Debug button to load a game
            if (GUILayout.Button("Load Game"))
            {
                saveLoadSystem.LoadGame(gameName);
            }

            // Debug button to delete the game
            if (GUILayout.Button("Delete Game"))
            {
                saveLoadSystem.DeleteGame(gameName);
            }
        }
    }
}
