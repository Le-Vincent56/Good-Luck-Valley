using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GoodLuckValley.Persistence.Editor
{
    [CustomEditor(typeof(SaveLoadSystem))]
    public class SaveLoadSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Create an object to use within the inspector
            SaveLoadSystem saveLoadSystem = (SaveLoadSystem)target;
            string gameName = saveLoadSystem.selectedData.Name;

            // Draw the default inspector
            DrawDefaultInspector();

            if(GUILayout.Button("New Game"))
            {
                saveLoadSystem.NewGame(1);
            }

            // Create a button to save the game
            if(GUILayout.Button("Save Game"))
            {
                saveLoadSystem.SaveGame();
            }

            // Create a button to load the game
            if(GUILayout.Button("Load Game"))
            {
                saveLoadSystem.LoadGame(gameName);
            }

            // Create a button to delete the game
            if(GUILayout.Button("Delete Game"))
            {
                saveLoadSystem.DeleteGame(gameName);
            }
        }
    }
}